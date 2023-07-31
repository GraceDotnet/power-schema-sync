# power-schema-sync
[![Nuget](https://img.shields.io/nuget/v/PowerSchemaSync)](https://www.nuget.org/packages/PowerSchemaSync/)

`power-schema-sync`是一款使用`.Net`开发的数据库结构同步工具，通过比对两个数据库的结构，生成同步结构所需执行的sql，其功能类似`Navicat`中的结构同步；不同的是本项目作为一个开源库，可直接集成到您现有的系统中，实现定时同步或监听某个事件来触发同步任务。

# 快速开始
### 安装
```
dotnet add package PowerSchemaSync
```

### 用法
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
# 特性
- [x] table sync
- [x] column ysnc
- [x] index ysnc
- [x] primary ysnc
- [x] view ysnc
- [ ] func sync
- [ ] stored procedure sync
# 支持的数据库

- [x] Mysql
- [ ] MS SqlServer
- [ ] Orecle
