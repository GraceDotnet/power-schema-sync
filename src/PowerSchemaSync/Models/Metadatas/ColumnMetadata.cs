using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSchemaSync.Models.Metadatas
{
    public class ColumnMetadata
    {
        public string Name { get; set; }

        public string COLUMN_TYPE { get; set; }

        public string IsNull { get; set; }

        public string DefaultValue { get; set; }

        public string Comment { get; set; }

        public string Extra { get; set; }

        public int ORDINAL_POSITION { get; set; }

        public string COLUMN_KEY { get; set; }

        public string DATA_TYPE { get; internal set; }

        public string Default
        {
            get
            {
                if (DefaultValue == null)
                {
                    return null;
                }

                if (DATA_TYPE.Contains("char")
                    || DATA_TYPE == "longtext"
                    || DATA_TYPE == "text")
                {
                    return $"'{DefaultValue}'";
                }

                return DefaultValue;
            }
        }
    }
}
