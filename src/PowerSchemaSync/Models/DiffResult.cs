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

        public DiffTable Source { get; set; }

        public DiffTable Target { get; set; }

        public List<string> SyncSqls { get; set; } = new List<string>();

        /// <summary>
        /// 需要操作的类型
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OprateEnum Operate { get; set; }
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
