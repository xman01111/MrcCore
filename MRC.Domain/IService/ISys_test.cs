using Chloe;
using Mrc.Data;
using MRC.Data.Enum;
using MRC.Entity;
using MRC.Service.Application;
using MRC.Service.Exceptions;
using MRC.ToolsAndEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MRC.Domain.IService
{
    public interface ISys_testService : IAppService
    {
        
        void Add(Sys_test model);
        void Update(Sys_test model);

    }

    public class Sys_testService : AppServiceBase<Sys_test>, ISys_testService
    {
        public Sys_testService(IDbContext dbContext, IServiceProvider services) : base(dbContext, services)
        {
        }
        public void Add(Sys_test model)
        {
            this.DbContext.DoWithTransaction(() =>
            {
                this.DbContext.Insert(model);
            });
        }
        public void Update(Sys_test model)
        {
            Sys_test updateModel = this.Query.Where(a => a.id == model.id).AsTracking().FirstOrDefault();
            updateModel = model;             
            this.DbContext.DoWithTransaction(() =>
            {
                this.DbContext.Update(updateModel);
                //this.Log(LogType.Update, "User", true, $"用户[{this.Session.UserId}]");
            });
        }


    }

   


}
