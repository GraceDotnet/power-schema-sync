# power-schema-sync
[![Nuget](https://img.shields.io/nuget/v/PowerSchemaSync)](https://www.nuget.org/packages/PowerSchemaSync/)

Compare database structures and generate SQL for structure synchronization，Easy integration into applications~

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
