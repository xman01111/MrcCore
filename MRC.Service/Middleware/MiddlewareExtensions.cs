using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace MRC.Service.Middleware
{
    //请求中间件拓展
    public static class RequestMiddlewareExtensions
    {
        public static IApplicationBuilder UseWriteLoginInfo(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoginInfoMiddleware>();
        }
    }


}
