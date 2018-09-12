﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MRC.Data.Models;
using MRC.Service.Helper;
using MRC.Data.Enum;

namespace MRC.Service.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class LoginAttribute : Attribute, IAuthorizationFilter
    {
        public LoginAttribute()
        {
           
        }

        public virtual void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            if (filterContext.Result != null)
                return;
            if(((bool)(filterContext.HttpContext.Items["islogin"] ?? false)) == true)
            {
                return;
            }
            HttpRequest httpRequest = filterContext.HttpContext.Request;
            if (httpRequest.IsAjaxRequest())
            {
                Result result = Result.CreateResult(ResultStatus.NotLogin, "未登录或登录超时，请重新登录");
                string json = JsonHelper.Serialize(result);
                ContentResult contentResult = new ContentResult() { Content = json };
                filterContext.Result = contentResult;
                return;
            }
            else
            {
                string url = filterContext.HttpContext.Content("~/Admin/Login");
                url = string.Concat(url, "?returnUrl=", httpRequest.Path);

                RedirectResult redirectResult = new RedirectResult(url);
                filterContext.Result = redirectResult;
                return;
            }
        }

        protected virtual bool SkipAuthorize(ActionDescriptor actionDescriptor)
        {
            bool skipAuthorize = actionDescriptor.FilterDescriptors.Where(a => a.Filter is AllowNotLoginAttribute).Any();
            if (skipAuthorize)
            {
                return true;
            }

            return false;
        }
    }
}
