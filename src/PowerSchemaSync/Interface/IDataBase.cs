using PowerSchemaSync.Models.Metadatas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSchemaSync.Interface
{
    public interface IDataBase
    {
        string AddColumnSql(string schema, string tableName, string columeName, string cOLUMN_TYPE, string isNull, string defaultValue, string comment, string position);

        /// <summary>
        /// 删除列的sql
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="tableName"></param>
        /// <param name="columeName"></param>
        /// <returns></returns>
        string DropColumnSql(string schema, string tableName, string columeName);
        
        /// <summary>
        /// 删除表的sql
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        string DropTableSql(string schema, string tableName);

        string ExportStructure(string schema);

        /// <summary>
        /// 获取库信息
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        IList<DataTableModel> GetSchemaMeta(string schema);
        string ModifyColumnSql(string schema, string tableName, string columeName, string cOLUMN_TYPE, string isNull, string defaultValue, string comment, string position);
    }
}
