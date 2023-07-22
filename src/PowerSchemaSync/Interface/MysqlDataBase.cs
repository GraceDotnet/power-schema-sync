using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using System.Data.Common;
using PowerSchemaSync.Models.Metadatas;

namespace PowerSchemaSync.Interface
{
    public class MysqlDataBase : IDataBase
    {
        readonly string connectionString;

        public MysqlDataBase(string connectionString)
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// 导出所有表结构
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public string ExportStructure(string schema)
        {
            var tables = getTables(schema);
            var sb = new StringBuilder();
            sb.AppendLine("SET NAMES utf8mb4;");
            sb.AppendLine("SET FOREIGN_KEY_CHECKS = 0;");

            using var conn = GetConnection();
            foreach (var item in tables)
            {
                var sql = $"SHOW CREATE TABLE `{schema}`.`{item}`;";
                var reader = conn.ExecuteReader(sql);

                if (reader.Read())
                {
                    sb.AppendLine("-- ----------------------------");
                    sb.AppendLine($"-- Table structure for {item}");
                    sb.AppendLine("-- ----------------------------");
                    sb.AppendLine(reader["Create Table"].ToString());
                    reader.Close();
                }
            }

            sb.AppendLine();
            sb.AppendLine("SET FOREIGN_KEY_CHECKS = 1;");
            return sb.ToString();
        }

        /// <summary>
        /// 初始化数据，所有操作之前必须先指定schema进行初始化
        /// </summary>
        /// <param name="schema"></param>
        /// <exception cref="NotImplementedException"></exception>

        /* 项目“PowerSchemaSync (net6.0)”的未合并的更改
        在此之前:
                public IList<Models.DataTableModel> GetSchemaMeta(string schema)
        在此之后:
                public IList<DataTableModel> GetSchemaMeta(string schema)
        */
        public IList<Models.Metadatas.DataTableModel> GetSchemaMeta(string schema)
        {

            /* 项目“PowerSchemaSync (net6.0)”的未合并的更改
            在此之前:
                        var dataTables = new List<Models.DataTableModel>();
            在此之后:
                        var dataTables = new List<DataTableModel>();
            */
            var dataTables = new List<Models.Metadatas.DataTableModel>();
            var tableNames = getTables(schema).OrderBy(x => x);
            using var conn = GetConnection();

            foreach (var tableName in tableNames)
            {
                var sql = $"SHOW CREATE TABLE `{schema}`.`{tableName}`;";
                var reader = conn.ExecuteReader(sql);
                string createTable = null;

                if (reader.Read())
                {
                    createTable = reader["Create Table"].ToString() + ";";
                    reader.Close();
                }

                var indexs = getIndexs(schema, tableName);
                var columes = getColumns(schema, tableName);

                dataTables.Add(new DataTableModel(schema, tableName, indexs, columes, createTable));
            }

            return dataTables;
        }

        private IList<ColumnMetadata> getColumns(string schema, string tableName)
        {
            var sql = @"select 
	                        * 
                        from 
	                        information_schema.columns
                        where 
 	                        TABLE_SCHEMA=@tableSchema and TABLE_NAME = @tableName 
                        order by ORDINAL_POSITION asc;";
            using var conn = GetConnection();

            var reader = conn.ExecuteReader(sql, new { tableSchema = schema, tableName });

            var columns = new List<ColumnMetadata>();
            while (reader.Read())
            {
                var column = new ColumnMetadata
                {
                    ORDINAL_POSITION = int.Parse(reader["ORDINAL_POSITION"].ToString()),
                    COLUMN_KEY = reader["COLUMN_KEY"].ToString(),
                    Name = reader["COLUMN_NAME"].ToString(),
                    COLUMN_TYPE = reader["COLUMN_TYPE"].ToString(),
                    DATA_TYPE = reader["DATA_TYPE"].ToString(),
                    IsNull = reader["IS_NULLABLE"].ToString(),
                    DefaultValue = reader["COLUMN_DEFAULT"] == DBNull.Value ? null : reader["COLUMN_DEFAULT"].ToString(),
                    Comment = reader["COLUMN_COMMENT"].ToString(),
                    Extra = reader["EXTRA"].ToString()
                };

                columns.Add(column);
            }

            reader.Close();
            return columns;
        }

        private List<IndexMetadata> getIndexs(string schema, string tableName)
        {
            using var conn = GetConnection();
            var sql = $"show keys from `{schema}`.`{tableName}`";
            using var reader = conn.ExecuteReader(sql);

            var indexs = new List<IndexMetadata>();
            IndexMetadata last = null;
            while (reader.Read())
            {
                string keyName = reader["Key_name"].ToString();

                if (last == null || keyName != last.IndexName)
                {
                    last = new IndexMetadata();
                    last.IndexName = keyName;
                    last.Columns.Add(reader["Column_name"].ToString());
                    last.NotUnique = reader["Non_unique"].ToString() == "1";
                    last.COMMENT = reader["Index_comment"].ToString();
                    last.IndexType = reader["Index_type"].ToString();
                    indexs.Add(last);
                }
                else
                {
                    // 表明这两个key在同一索引中
                    last.Columns.Add(reader["Column_name"].ToString());
                }
            }

            reader.Close();

            return indexs;
        }

        MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        private IEnumerable<string> getTables(string schema)
        {
            using var conn = GetConnection();
            var sql = $"show full tables from {schema} where Table_type = 'BASE TABLE'";
            return conn.Query<string>(sql);
        }

        /// <summary>
        /// 删除表
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public string DropTableSql(string schema, string tableName)
        {
            return $"DROP TABLE `{schema}`.`{tableName}`;";
        }

        /// <summary>
        /// 删除列
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="tableName"></param>
        /// <param name="columeName"></param>
        /// <returns></returns>
        public string DropColumnSql(string schema, string tableName, string columeName)
        {
            return $"ALTER TABLE `{schema}`.`{tableName}` DROP COLUMN `{columeName}`;";
        }

        /// <summary>
        /// 添加列的sql
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="tableName"></param>
        /// <param name="columeName"></param>
        /// <param name="columnType"></param>
        /// <param name="isNull"></param>
        /// <param name="defaultValue"></param>
        /// <param name="comment"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public string AddColumnSql(string schema, string tableName, string columeName, string columnType, string isNull, string defaultValue, string comment, string position)
        {
            return $"ALTER TABLE `{schema}`.`{tableName}` ADD COLUMN `{columeName}` {columnType} {(isNull == "YES" ? "NULL" : "NOT NULL")}{defaultValue}{comment}{position};";
        }

        /// <summary>
        /// 修改列的sql
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="tableName"></param>
        /// <param name="columeName"></param>
        /// <param name="columnType"></param>
        /// <param name="isNull"></param>
        /// <param name="defaultValue"></param>
        /// <param name="comment"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public string ModifyColumnSql(string schema, string tableName, string columeName, string columnType, string isNull, string defaultValue, string comment, string position)
        {
            return $"ALTER TABLE `{schema}`.`{tableName}` MODIFY COLUMN `{columeName}` {columnType} {(isNull == "YES" ? "NULL" : "NOT NULL")}{defaultValue}{comment}{position};";
        }
    }
}
