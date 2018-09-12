using Microsoft.AspNetCore.Http;
using MRC.Data;
using MRC.Service.Helper;
using MRC.Service.Tools;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MRC.Service.Middleware
{
    public class LoginInfoMiddleware
    {
        private readonly RequestDelegate _next;
        public LoginInfoMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public Task Invoke(HttpContext context)
        {
            string token = "";
            if (context.Request.Cookies.TryGetValue("MRCTOKEN", out token))
            {
                string userinfo = EncryptHelper.DesDecrypt(token, KeyTool.GetEncryptKey());
                string orginInfo = RedisHelper.Get(userinfo);
                if (orginInfo.IsNullOrEmpty()) return null;
                AdminSession userSession = JsonHelper.Deserialize<AdminSession>(orginInfo);
                if (context.GetClientIP() != userSession.LoginIP)
                {
                    context.Items["islogin"] = false;
                }
                else
                {
                    context.Items["user"] = userSession;
                    context.Items["islogin"] = true;
                }               
            }
            return this._next(context);
        }
    }
}
