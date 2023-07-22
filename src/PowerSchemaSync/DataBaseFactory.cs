using PowerSchemaSync.Interface;
using PowerSchemaSync.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSchemaSync
{
    public class DataBaseFactory
    {
        public static IDataBase GetDataBase(DataBaseType dataBaseType, string connString)
        {
            switch (dataBaseType)
            {
                case DataBaseType.MYSQL:
                    return new MysqlDataBase(connString);
                case DataBaseType.SQL_SERVER:
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
