using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSchemaSync.Models.Metadatas
{
    public class DataTableModel
    {
        public DataTableModel(string schema, string tableName, IList<IndexMetadata> indexs, IList<ColumnMetadata> columes, string createTable)
        {
            TableName = tableName;
            Indexs = indexs;
            Columes = columes;
            Schema = schema;
            CreateTable = createTable;
        }

        public string Schema { get; }

        public string TableName { get; }

        public IList<IndexMetadata> Indexs { get; }

        public IList<ColumnMetadata> Columes { get; }

        public string CreateTable { get; }
    }
}
