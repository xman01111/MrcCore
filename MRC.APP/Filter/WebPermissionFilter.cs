using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using MRC.Service.Authorization;
using MRC.Domain.IService;
using MRC.Data;

namespace MRC.APP
{
    public class WebPermissionFilter : PermissionFilter
    {
        public const string USER_PERMITS_CACHE_KEY = " UserPermission:userid";

        IMemoryCache _cache;

        public WebPermissionFilter(IMemoryCache cache)
        {
            this._cache = cache;
        }

        protected override bool HasExecutePermission(AuthorizationFilterContext filterContext, List<string> permissionCodes)
        {
            AdminSession user = filterContext.HttpContext.Items["user"] as AdminSession;
            if (user.AccountName == MRC.Entity.Sys_User.AdminAccountName)
                return true;
            List<string> usePermits = null;         
            string cacheKey = USER_PERMITS_CACHE_KEY + user.UserId;
            string cacheValue = RedisHelper.Get(cacheKey);
            if (cacheValue.IsNotNullOrEmpty())
            {
                usePermits=cacheValue.Split('|').ToList();
            }
            if (usePermits == null)
            {
                IUserService userService = filterContext.HttpContext.RequestServices.GetService(typeof(IUserService)) as IUserService;
                usePermits = userService.GetUserPermits(user.UserId);
                RedisHelper.Set(cacheKey, string.Join("|", usePermits));             
            }
            foreach (string permit in permissionCodes)
            {
                if (!usePermits.Any(a => a == permit))
                    return false;
            }
            return true;
        }
    }
}

