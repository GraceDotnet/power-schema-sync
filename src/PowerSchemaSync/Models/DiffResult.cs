using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PowerSchemaSync.Models
{
    public class DiffResult
    {
        /// <summary>
        /// 需要修改结构的表
        /// </summary>
        public List<DiffInfo> Tables { get; set; } = new List<DiffInfo>();
    }

    public class DiffInfo
    {
        public string TableName { get; set; }

        public List<DiffColumn> Columns { get; set; } = new List<DiffColumn>();

        public List<DiffIndex> Indexs { get; set; } = new List<DiffIndex>();

        /// <summary>
        /// 需要操作的类型
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OprateEnum Operate { get; set; }

        public string SyncSql { get; set; }

        [JsonIgnore]
        public IEnumerable<string> SyncSqls
        {
            get
            {
                return Columns.Select(c => c.SyncSql).Concat(Indexs.Select(x => x.SyncSql));
            }
        }
    }

    public class DiffIndex
    {
        /// <summary>
        /// 需要操作的类型
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OprateEnum Operate { get; set; }

        public string Name { get; set; }

        public string SyncSql { get; set; }
    }

    public class DiffColumn
    {
        /// <summary>
        /// 需要操作的类型
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OprateEnum Operate { get; set; }

        public string Name { get; set; }

        public string SyncSql { get; set; }
    }

    public enum OprateEnum
    {
        [Description("无")]
        None = 0,

        [Description("创建")]
        Created = 1,

        [Description("修改")]
        Edit = 2,

        [Description("删除")]
        Delete = 3
    }

    public class DiffTable
    {

    }
}
