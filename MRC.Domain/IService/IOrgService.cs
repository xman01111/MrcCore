
using Chloe;
using MRC.Entity;
using MRC.Service.Application;
using MRC.Service.Models;
using MRC.ToolsAndEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRC.Domain.IService
{
    public interface IOrgService : IAppService
    {
        List<Sys_Org> GetList(string keyword = "");
        void Add(AddOrgInput input);
        void Update(UpdateOrgInput input);
        void Delete(string id, string operatorId);
        List<Sys_OrgPermission> GetPermissions(string orgId);
        void SetPermission(string orgId, List<string> permissionList);
        List<Sys_Org> GetParentOrgs(int orgType);
        List<Sys_OrgType> GetOrgTypes();
        List<Sys_Org> GetListById(List<string> ids);
    }

    public class OrgService :AppServiceBase<Sys_Org>, IOrgService
    {
        public OrgService(IDbContext dbContext, IServiceProvider services) : base(dbContext, services)
        {

        }
        public List<Sys_Org> GetList(string keyword = "")
        {
            var q = this.Query.FilterDeleted();
            if (keyword.IsNotNullOrEmpty())
            {
                q = q.Where(a => a.Name.Contains(keyword));
            }

            var ret = q.ToList();
            return ret;
        }
        public void Add(AddOrgInput input)
        {
            this.InsertFromDto(input);
        }
        public void Update(UpdateOrgInput input)
        {
            this.UpdateFromDto(input);
        }

        public void Delete(string id, string operatorId)
        {
            this.SoftDelete(id, operatorId);
        }

        public List<Sys_OrgPermission> GetPermissions(string orgId)
        {
            return this.DbContext.Query<Sys_OrgPermission>().Where(t => t.OrgId == orgId).ToList();
        }
        public void SetPermission(string orgId, List<string> permissionList)
        {
            orgId.NotNullOrEmpty();

            List<Sys_OrgPermission> roleAuths = permissionList.Select(a => new Sys_OrgPermission()
            {
                Id = IdHelper.CreateStringSnowflakeId(),
                OrgId = orgId,
                PermissionId = a
            }).ToList();

            this.DbContext.DoWithTransaction(() =>
            {
                this.DbContext.Delete<Sys_OrgPermission>(a => a.OrgId == orgId);
                this.DbContext.InsertRange(roleAuths);
            });
        }
        public List<Sys_Org> GetParentOrgs(int orgType)
        {
            var orgTypeQuery = this.DbContext.Query<Sys_OrgType>().Where(a => a.Id == orgType);
            var q = this.Query.Where(a => Sql.Equals(a.OrgType, orgTypeQuery.First().ParentId));
            List<Sys_Org> ret = q.ToList();

            return ret;
        }
        public List<Sys_OrgType> GetOrgTypes()
        {
            return this.DbContext.Query<Sys_OrgType>().ToList();
        }
        public List<Sys_Org> GetListById(List<string> ids)
        {
            if (ids.Count == 0)
                return new List<Sys_Org>();

            return this.Query.FilterDeleted().Where(a => ids.Contains(a.Id)).ToList();
        }
    }

}
