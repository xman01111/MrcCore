using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.IO;
using MRC.AutoMapper;
using NLog.Extensions.Logging;
using NLog.Web;
using MRC.Data;
using MRC.Service;
using MRC.Service.Application;
using MRC.Service.Middleware;
namespace MRC.APP
{
    public class Startup
    {

        IHostingEnvironment _env;

        public Startup(IHostingEnvironment env)
        {
            this._env = env;

            var builder = new ConfigurationBuilder()
                      .SetBasePath(env.ContentRootPath)
                      .AddJsonFile(Path.Combine("configs", "appsettings.json"), optional: true, reloadOnChange: true)  // Settings for the application
                      .AddJsonFile(Path.Combine("configs", $"appsettings.{env.EnvironmentName}.json"), optional: true, reloadOnChange: true)
                      .AddEnvironmentVariables();                                              // override settings with environment variables set in compose.   
            Configuration = builder.Build();
            Globals.Configuration = Configuration;
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddDatabase(this.Configuration);
            services.RegisterAppServices(); /* 注册应用服务 */
            
            /* 缓存 */
            services.AddMemoryCache();
            services.AddResponseCaching();
            services.AddSession();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options =>
            {
            });

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
                options.Filters.Add(typeof(WebPermissionFilter));
            }).AddControllersAsServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            /* NLog */
            this._env.ConfigureNLog(Path.Combine("configs", "nlog.config"));
            loggerFactory.AddNLog();
            app.AddNLogWeb();

            Globals.Services = app.ApplicationServices;

            //RedisHelper.Initialization(
            //csredis: new CSRedis.CSRedisClient("192.168.1.198:6379,pass=123,defaultDatabase=13,poolsize=50,ssl=false,writeBuffer=10240,prefix=key"),
            //serialize: value => Newtonsoft.Json.JsonConvert.SerializeObject(value),
            //deserialize: (data, type) => Newtonsoft.Json.JsonConvert.DeserializeObject(data, type));

            MrcMapper.InitializeMap(); /* 初始化 AutoMapper */

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseSession();
            app.UseResponseCaching();
            app.UseWriteLoginInfo();//登陆信息
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                name: "areaname",
                template: "{Admin:exists}/{controller=Home}/{action=Index}/{id?}");
                routes.MapAreaRoute(
                    name: "Admin",
                    areaName: "Admin",
                    template: "Admin/{controller=Home}/{action=Index}"
                    );
            });
        }
    }
}
