# power-schema-sync
[![Nuget](https://img.shields.io/nuget/v/PowerSchemaSync)](https://www.nuget.org/packages/PowerSchemaSync/)

`power-schema-sync` is a database structure synchronization tool developed using `.Net`, which compares the structure of the two databases to generate the SQL required to synchronize the structure, which functions similarly to the structure synchronization in `Navicat`; The difference is that this project is an open source library that can be directly integrated into your existing system to implement regular synchronization or listen for an event to trigger a synchronization task.

# Quick Start
### install
```
dotnet add package PowerSchemaSync
```

### usages
```c#
IDataBase dbSoruce = DataBaseFactory.GetDataBase(DataBaseType.MYSQL, "connString1");
IDataBase dbTarget = DataBaseFactory.GetDataBase(DataBaseType.MYSQL, "connString2");

// 1.获取整个库的表结构创建sql
var sqlStructure = dbSoruce.ExportStructure("test");
Console.WriteLine(sqlStructure);

// 2.结构比较获取同步结构的sql
var diff = new DiffUtility(dbSoruce, "test", dbTarget, "test").Diff();

Console.WriteLine("------------新建-----------");
foreach (var sqls in diff.Tables.Where(x => x.Operate == OprateEnum.Created).Select(x => x.SyncSqls))
{
    foreach (var sql in sqls)
    {
        Console.WriteLine(sql);
    }
}

Console.WriteLine("------------删除-----------");
foreach (var sqls in diff.Tables.Where(x => x.Operate == OprateEnum.Delete).Select(x => x.SyncSqls))
{
    foreach (var sql in sqls)
    {
        Console.WriteLine(sql);
    }
}

Console.WriteLine("------------修改-----------");
foreach (var sqls in diff.Tables.Where(x => x.Operate == OprateEnum.Edit).Select(x => x.SyncSqls))
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
