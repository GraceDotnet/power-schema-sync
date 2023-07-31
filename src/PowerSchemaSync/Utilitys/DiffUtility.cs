using PowerSchemaSync.Interface;
using PowerSchemaSync.Models;
using PowerSchemaSync.Models.Metadatas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PowerSchemaSync.Utilitys
{
    /// <summary>
    /// 结构比较工具
    /// </summary>
    public class DiffUtility
    {
        string soruceSchema;
        string targetSchema;

        public IDataBase Source { get; }

        public IDataBase Target { get; }

        public IList<DataTableModel> SourceTables { get; private set; }

        public IList<DataTableModel> TargetTables { get; private set; }

        public DiffUtility(IDataBase dbSoruce, string soruceSchema, IDataBase dbTarget, string targetSchema)
        {
            this.soruceSchema = soruceSchema;
            this.targetSchema = targetSchema;
            Source = dbSoruce;
            Target = dbTarget;
        }

        /// <summary>
        /// 结构比较获取同步结构的sql
        /// </summary>
        /// <returns></returns>
        public DiffResult Diff()
        {
            SourceTables = Source.GetSchemaMeta(soruceSchema);
            TargetTables = Source.GetSchemaMeta(targetSchema);

            var res = new DiffResult();

            // 1.获取两个库中并集
            var unionTables = SourceTables.Select(x => x.TableName).Union(TargetTables.Select(x => x.TableName));
            foreach (var tableName in unionTables)
            {
                var sourceTable = SourceTables.FirstOrDefault(x => x.TableName == tableName);
                var targetTable = TargetTables.FirstOrDefault(x => x.TableName == tableName);

                var diff = new DiffInfo();
                diff.TableName = tableName;

                if (sourceTable == null)
                {
                    var dropSql = Target.DropTableSql(targetTable!.Schema, tableName);
                    diff.SyncSql = dropSql;
                    diff.Operate = OprateEnum.Delete;
                    res.Tables.Add(diff);
                    continue;
                }

                if (targetTable == null)
                {
                    diff.SyncSql = sourceTable.CreateTable;
                    diff.Operate = OprateEnum.Created;
                    res.Tables.Add(diff);
                    continue;
                }

                // 两库中均存在该表
                diff.Operate = OprateEnum.None;

                diffColumns(ref diff, sourceTable, targetTable);
                diffIndex(ref diff, sourceTable, targetTable);

                res.Tables.Add(diff);
            }

            return res;
        }

        private void diffColumns(ref DiffInfo diff, DataTableModel sourceTable, DataTableModel targetTable)
        {
            // 1.获取两个库中并集
            var unionColumes = sourceTable.Columes.Select(x => x.Name).Union(targetTable.Columes.Select(x => x.Name));

            foreach (var columeName in unionColumes)
            {
                var sourceColume = sourceTable.Columes.FirstOrDefault(x => x.Name == columeName);
                var targetColume = targetTable.Columes.FirstOrDefault(x => x.Name == columeName);

                if (sourceColume == null)
                {
                    var sql = Target.DropColumnSql(targetTable.Schema, targetTable.TableName, columeName);
                    diff.Columns.Add(new DiffColumn
                    {
                        Name = columeName,
                        Operate = OprateEnum.Delete,
                        SyncSql = sql,
                    });
                    continue;
                }

                // 默认值
                string defaultValue = $"{(sourceColume.DefaultValue == null ? sourceColume.IsNull == "NO" ? null : " DEFAULT NULL" : " DEFAULT " + (sourceColume.DefaultValue == "" ? "''" : sourceColume.DefaultValue))}";
                if (!string.IsNullOrEmpty(sourceColume.Extra)
                    && (sourceColume.COLUMN_TYPE == "datetime" || sourceColume.Extra.ToLower() == "auto_increment"))
                {
                    defaultValue = " " + sourceColume.Extra.ToUpper();
                }
                else
                {
                    // 警告：未知的特性
                }

                // 确定字段的位置
                var index = sourceTable.Columes.IndexOf(sourceTable.Columes.First(x => x.Name == columeName));
                var position = "";
                if (index == 0)
                    position = " FIRST";
                else
                    position = $" AFTER `{sourceTable.Columes[index - 1].Name}`";

                var comment = $"{(string.IsNullOrEmpty(sourceColume.Comment) ? null : $" COMMENT '{sourceColume.Comment}'")}";
                if (targetColume == null)
                {
                    var sql = Target.AddColumnSql(targetTable.Schema, targetTable.TableName, columeName, sourceColume.COLUMN_TYPE, sourceColume.IsNull, defaultValue, comment, position);
                    diff.Columns.Add(new DiffColumn
                    {
                        Name = columeName,
                        Operate = OprateEnum.Created,
                        SyncSql = sql,
                    });
                    continue;
                }

                // 比较两个字段的定义，任何一个不同就需要按源表的字段进行覆盖
                if (targetColume.DefaultValue != sourceColume.DefaultValue
                    || targetColume.Comment != sourceColume.Comment
                    || targetColume.Extra != sourceColume.Extra
                    || targetColume.IsNull != sourceColume.IsNull || !checkType(targetColume, sourceColume))
                {
                    var sql = Target.ModifyColumnSql(targetTable.Schema, targetTable.TableName, columeName, sourceColume.COLUMN_TYPE, sourceColume.IsNull, defaultValue, comment, position);
                    diff.Columns.Add(new DiffColumn
                    {
                        Name = columeName,
                        Operate = OprateEnum.Edit,
                        SyncSql = sql,
                    });
                }
            }

            if (diff.Columns.Any(x => x.Operate != OprateEnum.None))
            {
                diff.Operate = OprateEnum.Edit;
            }
        }

        // 从8.0.17版本开始，TINYINT, SMALLINT, MEDIUMINT, INT, and BIGINT 类型的显示宽度将失效。
        string[] IgnoreSize = new string[] { "tinyint", "smallint", "mediumint", "int", "bigint", };

        bool checkType(ColumnMetadata column1, ColumnMetadata column2)
        {
            if (column1.ORDINAL_POSITION != column2.ORDINAL_POSITION)
            {
                return false;
            }

            if (column1.COLUMN_TYPE == column2.COLUMN_TYPE)
                return true;

            if (column1.DATA_TYPE != column2.DATA_TYPE)
                return false;

            return IgnoreSize.Contains(column1.DATA_TYPE);
        }

        private void diffIndex(ref DiffInfo diff, DataTableModel sourceTable, DataTableModel targetTable)
        {
            // 1.获取两个库中并集
            var unionIndexs = sourceTable.Indexs.Select(x => x.IndexName).Union(targetTable.Indexs.Select(x => x.IndexName));

            // TODO: 如果索引的字段和类型相同，名称不同，则只修改名称即可
            foreach (var indexName in unionIndexs)
            {
                var sourceIndex = sourceTable.Indexs.FirstOrDefault(x => x.IndexName == indexName);
                var targetIndex = targetTable.Indexs.FirstOrDefault(x => x.IndexName == indexName);
                var indexTypeWithName = $"INDEX `{indexName}`";
                if (indexName == "PRIMARY")
                {
                    indexTypeWithName = "PRIMARY KEY";
                }

                if (sourceIndex == null)
                {
                    var deleteSql = $"ALTER TABLE `{targetTable.Schema}`.`{targetTable.TableName}` DROP {indexTypeWithName};";
                    diff.Indexs.Add(new DiffIndex
                    {
                        Name = indexName,
                        Operate = OprateEnum.Delete,
                        SyncSql = deleteSql
                    });
                    continue;
                }

                var comment = $"{(string.IsNullOrEmpty(sourceIndex.COMMENT) ? null : $" COMMENT '{sourceIndex.COMMENT}'")}";
                if (targetIndex == null)
                {
                    if (sourceIndex.IndexType == "FULLTEXT")
                    {
                        // 查看源表全文索引使用的解析器
                        var parser = getFTParser(sourceTable.CreateTable, indexName);
                        diff.Indexs.Add(new DiffIndex
                        {
                            Name = indexName,
                            Operate = OprateEnum.Created,
                            SyncSql = $"ALTER TABLE `{targetTable.Schema}`.`{targetTable.TableName}` ADD FULLTEXT {indexTypeWithName}({sourceIndex.ColumnsJoined}){parser}{comment};"
                        });
                    }
                    else
                    {
                        diff.Indexs.Add(new DiffIndex
                        {
                            Name = indexName,
                            Operate = OprateEnum.Created,
                            SyncSql = $"ALTER TABLE `{targetTable.Schema}`.`{targetTable.TableName}` ADD {indexTypeWithName}({sourceIndex.ColumnsJoined}) USING {sourceIndex.IndexType}{comment};"
                        });
                    }

                    continue;
                }

                // 比较两个字段的定义，任何一个不同就需要按源表的字段进行覆盖
                if (sourceIndex.ColumnsJoined != targetIndex.ColumnsJoined
                    || sourceIndex.COMMENT != targetIndex.COMMENT
                    || sourceIndex.IndexType != targetIndex.IndexType
                    || sourceIndex.NotUnique != targetIndex.NotUnique)
                {
                    var dropAndRebuild = new StringBuilder($"ALTER TABLE `{targetTable.Schema}`.`{targetTable.TableName}` DROP {indexTypeWithName},");

                    if (sourceIndex.IndexType == "FULLTEXT")
                    {
                        // 查看源表全文索引使用的解析器
                        var parser = getFTParser(sourceTable.CreateTable, indexName);
                        dropAndRebuild.Append($"ADD FULLTEXT {indexTypeWithName}({sourceIndex.ColumnsJoined}){parser}{comment};");
                    }
                    else
                        dropAndRebuild.Append($"ADD {indexTypeWithName}({sourceIndex.ColumnsJoined}) USING {sourceIndex.IndexType}{comment};");

                    diff.Indexs.Add(new DiffIndex
                    {
                        SyncSql = dropAndRebuild.ToString(),
                        Operate = OprateEnum.Edit,
                        Name = indexName,
                    });
                }
            }

            if (diff.Indexs.Any(x => x.Operate != OprateEnum.None))
            {
                diff.Operate = OprateEnum.Edit;
            }
        }

        private string getFTParser(string createTable, string indexName)
        {
            // FULLTEXT KEY `enter_car_plate` (`enter_car_plate`) /*!50100 WITH PARSER `ngram` */ ,
            foreach (var item in createTable.Split('\n'))
            {
                if (item.Trim().StartsWith($"FULLTEXT KEY `{indexName}`"))
                {
                    var regex = @"WITH PARSER `.+`";
                    var res = Regex.Matches(item, regex);

                    if (res.Count > 0)
                    {
                        return " " + res[0].Value;
                    }

                    break;
                }
            }

            return null;
        }
    }
}
