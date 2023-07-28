using Microsoft.AspNetCore.Mvc;
using OnlineWebApp.Models;
using PowerSchemaSync.Models;
using System.Diagnostics;

namespace OnlineWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PreSync()
        {
            return View();
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
        public IActionResult SyncCheck(string s, string ss, DataBaseType st, string t, string ts, DataBaseType tt)
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}