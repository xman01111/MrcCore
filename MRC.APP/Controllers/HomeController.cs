using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MRC.APP.Models;
using MRC.APP;
using MRC.Domain;
using MRC.Domain.IService;
using MRC.Entity;
using MRC.Service.Application;
using System.Reflection;
using CSRedis;

namespace MRC.APP.Controllers
{
    public class HomeController :WebBaseController<IUserService>
    {
        public readonly IUserService _userService;
        public readonly ISys_testService _sys_TestService;
        public HomeController(IUserService userService,ISys_testService sys_TestService)
        {
            this._userService = userService;
            this._sys_TestService = sys_TestService;
        }
        
        public IActionResult Index()
        {
            string key = "TestAsyncKey:";

                try
                {
                    RedisClient redis = new RedisClient("139.199.186.71", 6379);
                    redis.Auth("xman01111");
                    redis.Select(13);
                    var keys= redis.Scan(18);
                    var allKey=redis.Keys("*");
                    string result = redis.Ping();
                    redis.Quit();
                }
                catch (Exception ex)
                {
                    string s = ex.Message;
                }
            return View(); 
        }
    }
}
