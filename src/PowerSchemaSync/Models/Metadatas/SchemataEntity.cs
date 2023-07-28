using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSchemaSync.Models.Metadatas
{
    public class SchemataEntity
    {
        public string SCHEMA_NAME { get; set; }

        public string DEFAULT_CHARACTER_SET_NAME { get; set; }

        public string DEFAULT_COLLATION_NAME { get; set; }

        public string SQL_PATH { get; set; }
    }
}
