//using Chloe;
//using MRC.Entity;
//using MRC.Service.Application;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace MRC.Domain.IService
//{
//    public interface IPostService : IAppService
//    {
//        List<Sys_Post> GetList(string keyword = "");
//        void Add(AddPostInput input);
//        void Update(UpdatePostInput input);
//        void Delete(string id);
//        List<Sys_Post> GetListByOrgId(List<string> orgIds);
//    }

//    public class PostService : AdminAppService<Sys_Post>, IPostService
//    {
//        public List<Sys_Post> GetList(string keyword = "")
//        {
//            var q = this.Query.FilterDeleted();
//            if (keyword.IsNotNullOrEmpty())
//            {
//                q = q.Where(a => a.Name.Contains(keyword));
//            }

//            var ret = q.OrderBy(a => a.OrgId).ToList();
//            return ret;
//        }
//        public void Add(AddPostInput input)
//        {
//            this.InsertFromDto(input);
//        }
//        public void Update(UpdatePostInput input)
//        {
//            this.UpdateFromDto(input);
//        }

//        public void Delete(string id)
//        {
//            this.SoftDelete(id);
//        }

//        public List<Sys_Post> GetListByOrgId(List<string> orgIds)
//        {
//            if (orgIds.Count == 0)
//                return new List<Sys_Post>();

//            return this.Query.FilterDeleted().Where(a => orgIds.Contains(a.OrgId)).ToList();
//        }
//    }

//}
