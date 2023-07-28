using Microsoft.AspNetCore.Mvc;
using OnlineWebApp.Filters;
using PowerSchemaSync;
using PowerSchemaSync.Interface;
using PowerSchemaSync.Models;
using PowerSchemaSync.Models.Metadatas;
using PowerSchemaSync.Utilitys;

namespace OnlineWebApp.Controllers
{
    [ServiceFilter(typeof(CustomExceptionFilterAttribute))]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SyncController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<SchemataEntity> Schemas(DataBaseType dbType, string connStr)
        {
            IDataBase dataBase = DataBaseFactory.GetDataBase(dbType, connStr);
            return dataBase.GetSchemas();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s">源连接串</param>
        /// <param name="ss">源schema</param>
        /// <param name="st">源数据库类型</param>
        /// <param name="t">目标连接串</param>
        /// <param name="ts">目标schema</param>
        /// <param name="tt">目标数据库类型</param>
        /// <returns></returns>
        [HttpGet]
        public DiffResult Diff(string s, string ss, DataBaseType st, string t, string ts, DataBaseType tt)
        {
            IDataBase dbSoruce = DataBaseFactory.GetDataBase(st, s);
            IDataBase dbTarget = DataBaseFactory.GetDataBase(tt, t);

            // 2.结构比较获取同步结构的sql
            var diff = new DiffUtility(dbSoruce, ss, dbTarget, ts).Diff();
            return diff;
        }

        [HttpPost]
        public ExecResult Exec([FromQuery] string t, [FromQuery] string ts, [FromQuery] DataBaseType tt, [FromBody] DiffResult diff)
        {
            IDataBase dbTarget = DataBaseFactory.GetDataBase(tt, t);
            return dbTarget.Exec(ts, diff, new ExecOptions());
        }
    }
}
