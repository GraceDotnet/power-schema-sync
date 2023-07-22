# power-schema-sync
Compare database structures and generate SQL for structure synchronization

# Quick Start
```c#
IDataBase dbSoruce = DataBaseFactory.GetDataBase(DataBaseType.MYSQL, "connString1");
IDataBase dbTarget = DataBaseFactory.GetDataBase(DataBaseType.MYSQL, "connString2");

// 1.获取整个库的表结构创建sql
var sqlStructure = dbSoruce.ExportStructure("test");
Console.WriteLine(sqlStructure);

// 2.结构比较获取同步结构的sql
var diff = new DiffUtility(dbSoruce, "test", dbTarget, "test").Diff();

Console.WriteLine("------------需要新建的表-----------");
foreach (var sqls in diff.CreateTables.Select(x => x.SyncSqls))
{
    foreach (var sql in sqls)
    {
        Console.WriteLine(sql);
    }
}

Console.WriteLine("------------需要删除的表-----------");
foreach (var sqls in diff.DeleteTables.Select(x => x.SyncSqls))
{
    foreach (var sql in sqls)
    {
        Console.WriteLine(sql);
    }
}

Console.WriteLine("------------需要修改的字段或索引-----------");
foreach (var sqls in diff.EditTables.Select(x => x.SyncSqls))
{
    foreach (var sql in sqls)
    {
        Console.WriteLine(sql);
    }
}

Console.ReadLine();
```
# Features
- [x] table sync
- [x] column ysnc
- [x] index ysnc
- [x] primary ysnc
- [x] view ysnc
- [ ] func sync
- [ ] stored procedure sync
# Supported Databases

- [x] Mysql
- [ ] MS SqlServer
- [ ] Orecle
