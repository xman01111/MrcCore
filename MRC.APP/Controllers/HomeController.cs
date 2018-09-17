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
using Microsoft.AspNetCore.Http;
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
            ViewBag.ip = this.HttpContext.GetClientIP();
            var LastOne = this.CreateService<IRedPacketService>().GetLast();
            return View(LastOne);
        }



    }
}
