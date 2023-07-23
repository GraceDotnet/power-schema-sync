# power-schema-sync
[![Nuget](https://img.shields.io/nuget/v/PowerSchemaSync)](https://www.nuget.org/packages/PowerSchemaSync/)

比对两个数据库的结构，生成同步结构所需执行的sql，本项目作为一个开源库，可直接集成到您现有的系统中。由您的程序决定何时同步，或基于本库开发出您自己的数据库结构同步工具~

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
