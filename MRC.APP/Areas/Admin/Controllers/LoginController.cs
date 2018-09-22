using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MRC.Data;
using MRC.Data.Enum;
using MRC.Domain.IService;
using MRC.Entity;
using MRC.Service.Authorization;

namespace MRC.APP.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoginController : WebBaseController
    {
        [AllowNotLogin]
        public IActionResult Index()
        { 
            return View();
        }
        [HttpPost]
        [AllowNotLogin]
        public ActionResult Login(string loginName, string password/*经过md5加密后的密码*/, string verifyCode)
        {
            //if (verifyCode.IsNullOrEmpty())
            //    return this.FailedMsg("请输入验证码");
            if (loginName.IsNullOrEmpty() || password.IsNullOrEmpty())
                return this.FailedMsg("用户名/密码不能为空");
            //string code = WebHelper.GetSession<string>(VerifyCodeKey);
            //WebHelper.RemoveSession(VerifyCodeKey);
            //if (code.IsNullOrEmpty() || code.ToLower() != verifyCode.ToLower())
            //{
            //    return this.FailedMsg("验证码错误，请重新输入");
            //}

            loginName = loginName.Trim();
            var accountAppService = this.CreateService<IAccountService>();
            string ip = this.HttpContext.GetClientIP();
            Sys_User user;
            string msg;
            if (!accountAppService.CheckLogin(loginName, password, out user, out msg))
            {            
                return this.FailedMsg(msg);
            }
            AdminSession session = new AdminSession();
            session.UserId = user.Id;
            session.AccountName = user.AccountName;
            session.Name = user.Name;
            session.LoginIP = ip;
            session.IsAdmin = user.AccountName.ToLower() =="admin";
            try
            {
                this.CurrentSession = session;
            }
            catch (Exception ex)
            {

                throw;
            }
            
            
             
            // this.CreateService<ISysLogAppService>().LogAsync(user.Id, user.Name, ip, LogType.Login, moduleName, true, "登录成功");
            return this.SuccessMsg(msg);
        }
    }


}