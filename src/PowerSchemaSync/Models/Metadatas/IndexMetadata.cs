using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSchemaSync.Models.Metadatas
{
    public class IndexMetadata
    {
        public List<string> Columns { get; set; } = new List<string>();

        public string IndexName { get; set; }

        /// <summary>
        /// 0表示unique，1表示普通索引
        /// </summary>
        public bool NotUnique { get; set; }

        public string COMMENT { get; internal set; }

        public string ColumnsJoined => string.Join(",", Columns.Select(x => $"`{x}`"));

        public string IndexType { get; internal set; }
    }
}
