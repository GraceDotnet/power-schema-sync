using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSchemaSync.Models
{
    public class DiffResult
    {
        /// <summary>
        /// 需要修改结构的表
        /// </summary>
        public List<DiffInfo> EditTables { get; set; } = new List<DiffInfo>();

        /// <summary>
        /// 需要创建的表
        /// </summary>
        public List<DiffInfo> CreateTables { get; set; } = new List<DiffInfo>();

        /// <summary>
        /// 需要删除的表
        /// </summary>
        public List<DiffInfo> DeleteTables { get; set; } = new List<DiffInfo>();

        /// <summary>
        /// 相同的表
        /// </summary>
        public List<DiffInfo> SameTables { get; set; } = new List<DiffInfo>();
    }

    public class DiffInfo
    {
        public string TableName { get; set; }

        public DiffTable Source { get; set; }

        public DiffTable Target { get; set; }

        public List<string> SyncSqls { get; set; } = new List<string>();
    }

    public class DiffTable
    {

    }
}
