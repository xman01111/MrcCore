
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using MRC.Service.Application;
using MRC.Service.AttributeLib;
using MRC.Data;
using MRC.Service.Helper;
using MRC.Service.Tools;

namespace MRC.APP
{
    public abstract class WebBaseController<TService> : WebBaseController
    {
        TService _service;
        protected TService Service
        {
            get
            {
                if (this._service == null)
                    this._service = this.CreateService<TService>();

                return this._service;
            }
        }
    }
    public abstract class WebBaseController : BaseController
    {
        [Disposable]
        AppServiceFactory _appServicesFactory;
        AdminSession _session;
        IAppServiceFactory AppServicesFactory
        {
            get
            {
                if (this._appServicesFactory == null)
                    this._appServicesFactory = new AppServiceFactory(this.HttpContext.RequestServices, this.CurrentSession);
                return this._appServicesFactory;
            }
        }
        [NonAction]
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ObsoleteApiAttribute obsoleteAttr = filterContext.ActionDescriptor.FilterDescriptors.Where(a => a.Filter is ObsoleteApiAttribute).Select(a => a.Filter).FirstOrDefault() as ObsoleteApiAttribute;

            if (obsoleteAttr == null)
            {
                obsoleteAttr = filterContext.Controller.GetType().GetCustomAttributes<ObsoleteApiAttribute>().FirstOrDefault() as ObsoleteApiAttribute;
            }

            if (obsoleteAttr != null)
            {
                filterContext.Result = this.FailedMsg(obsoleteAttr.Message);
            }

            if (this.CurrentSession==null)
            {
                obsoleteAttr = filterContext.Controller.GetType().GetCustomAttributes<ObsoleteApiAttribute>().FirstOrDefault() as ObsoleteApiAttribute;
            }
            base.OnActionExecuting(filterContext);
        }

        //已通过中间件 将登录信息保存在 HttpContext.Items集合中
        public AdminSession CurrentSession
        {
            get
            {
                if(this._session != null)
                    return this._session;
                AdminSession userSession = null;
                if (((bool)(this.HttpContext.Items["islogin"] ?? false)) == true)
                {
                    userSession = this.HttpContext.Items["user"] as AdminSession;
                }
                this._session = userSession;
                return userSession;
            }
            set
            {
                AdminSession session = value;
                if (session == null)
                {
                    return;
                }
                var token = Guid.NewGuid().ToString();
                RedisHelper.Set(token,session.Serialize());
                this.HttpContext.Response.Cookies.Append("MRCTOKEN", token, new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(30)
                });
                this.HttpContext.Items["islogin"] = true;
                this.HttpContext.Items["user"]= session;
                if (this._appServicesFactory != null)
                {
                    this._appServicesFactory.Session = session;
                }
                this._session = session;
            }
        }
        protected T CreateService<T>()
        {
            return this.AppServicesFactory.CreateService<T>();
        }
    }
}