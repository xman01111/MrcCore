using Chloe;
using MRC.Data;
using MRC.ToolsAndEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MRC.Service.Application
{
    public abstract class AppServiceBase:IDisposable
    {
        public IDbContext DbContext { get; set; }
        public IServiceProvider Services { get; set; }
        protected AppServiceBase(IDbContext dbContext) : this(dbContext, null)
        {
        }
        protected AppServiceBase(IDbContext dbContext, IServiceProvider services)
        {
            this.DbContext = dbContext;
            this.Services = services;
        }

        /// <summary>
        /// 创建一个实体。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">为 null 则使用 IdHelper.CreateSnowflakeId()</param>
        /// <returns></returns>
        protected T CreateEntity<T>(object id = null, object creatorId = null) where T : new()
        {
            T entity = new T();
            Type entityType = typeof(T);

            entity.SetPropertyValue("Id", id ?? IdHelper.CreateSnowflakeId().ToString());
            entity.SetPropertyValue("CreationTime", DateTime.Now);

            if (creatorId != null)
                entity.SetPropertyValue("CreateUserId", creatorId);

            PropertyInfo prop_IsDeleted = entityType.GetProperty("IsDeleted");
            if (prop_IsDeleted != null)
            {
                prop_IsDeleted.SetValue(entity, false);
            }

            return entity;
        }


        protected void SoftDelete<T>(object id, object operatorId)
        {
            id.NotNullOrEmpty();
            this.DbContext.SoftDelete<T>(id, operatorId);
        }


        public void Dispose()
        {
            if (this.DbContext != null)
            {
                this.DbContext.Dispose();
            }
            this.Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {
        }
    }
    public class AppServiceBase<TEntity> : AppServiceBase
    {
        IQuery<TEntity> _query;

        protected AppServiceBase(IDbContext dbContext) : this(dbContext, null)
        {
        }
        protected AppServiceBase(IDbContext dbContext, IServiceProvider services) : base(dbContext, services)
        {
        }

        protected IQuery<TEntity> Query
        {
            get
            {
                if (this._query == null)
                    this._query = this.DbContext.Query<TEntity>();
                return this._query;
            }
        }


        protected TEntity InsertFromDto<Dto>(Dto dto)
        {
            if (dto is ValidationModel)
            {
                ValidationModel input = (ValidationModel)((object)dto);
                input.Validate();
            }

            return this.DbContext.InsertFromDto<TEntity, Dto>(dto);
        }
        protected int UpdateFromDto<Dto>(Dto dto)
        {
            if (dto is ValidationModel)
            {
                ValidationModel input = (ValidationModel)((object)dto);
                input.Validate();
            }

            return this.DbContext.UpdateFromDto<TEntity, Dto>(dto);
        }
        protected void Delete(object id)
        {
            id.NotNullOrEmpty();
            this.DbContext.DeleteByKey<TEntity>(id);
        }
        protected void SoftDelete(object id, object operatorId)
        {
            id.NotNullOrEmpty();
            this.DbContext.SoftDelete<TEntity>(id, operatorId);
        }
    }

}
