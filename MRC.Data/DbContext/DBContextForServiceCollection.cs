using Chloe;
using Microsoft.Extensions.Configuration;
using MRC.Data;
using System;
using System.Collections.Generic;
using System.Text;
namespace Microsoft.Extensions.DependencyInjection
{
    public static class DBContextForServiceCollection
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfigurationRoot config)
        {
            string connString = Globals.Configuration["db:ConnString"];
            string dbType = Globals.Configuration["db:DbType"];

            IDbContextFactory dbContextFactory = new DefaultDbContextFactory(dbType, connString);
            services.AddSingleton<IDbContextFactory>(dbContextFactory);
            services.AddScoped<IDbContext>(a =>
            {
                return a.GetService<IDbContextFactory>().CreateContext();
            });

            return services;
        }
    }
}
