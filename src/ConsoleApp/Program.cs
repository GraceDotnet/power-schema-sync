using PowerSchemaSync;
using PowerSchemaSync.Interface;
using PowerSchemaSync.Models;
using PowerSchemaSync.Utilitys;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IDataBase dbSoruce = DataBaseFactory.GetDataBase(DataBaseType.MYSQL, "connString1");
            IDataBase dbTarget = DataBaseFactory.GetDataBase(DataBaseType.MYSQL, "connString2");

            //// 1.获取整个库的表结构创建sql
            var sqlStructure = dbSoruce.ExportStructure("bxparking");
            Console.WriteLine(sqlStructure);

            // 2.结构比较获取同步结构的sql
            var diff = new DiffUtility(dbSoruce, "bxparking", dbTarget, "bxparking").Diff();

            Console.WriteLine("------------新建-----------");
            foreach (var sqls in diff.CreateTables.Select(x => x.SyncSqls))
            {
                foreach (var sql in sqls)
                {
                    Console.WriteLine(sql);
                }
            }

            Console.WriteLine("------------删除-----------");
            foreach (var sqls in diff.DeleteTables.Select(x => x.SyncSqls))
            {
                foreach (var sql in sqls)
                {
                    Console.WriteLine(sql);
                }
            }

            Console.WriteLine("------------修改-----------");
            foreach (var sqls in diff.EditTables.Select(x => x.SyncSqls))
            {
                foreach (var sql in sqls)
                {
                    Console.WriteLine(sql);
                }
            }

            Console.ReadLine();
        }
    }
}