using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MRC.Domain;
using MRC.Domain.IService;
using MRC.Entity;
using MRC.APP;
using MRC.ToolsAndEx;
using MRC.Service.Authorization;
using MRC.Service.Helper;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Data.SqlClient;
using MRC.Data;

namespace MRC.APP.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController :WebBaseController
    {

        private readonly IUserService  _userService;
        private readonly ISys_testService _sys_TestService;
        private IHostingEnvironment _hostingEnvironment;
        public HomeController(IUserService userService,ISys_testService sys_TestService, IHostingEnvironment environment)
        {
            this._userService = userService;
            this._sys_TestService = sys_TestService;
            this._hostingEnvironment = environment;
        }
        
        [Login]
        public IActionResult Index()
        {
            return View();
        }


        [Login]
        public IActionResult cosole()
        {
            return View();
        }


        [Login]
        [Permission("system.user.add")]
        public IActionResult add()
        {
            return View();
        }

        public IActionResult DBback()
        {
            IOHelper.CreateDirectory(Path.Combine(_hostingEnvironment.WebRootPath,"/DBBackUp/"));
            SqlParameter[] para_sys = new SqlParameter[]
            {
                new SqlParameter ("@BKPATH", Path.Combine(_hostingEnvironment.WebRootPath,"/DBBackUp/") + DateTime.Now.ToString("yyyyMMddHHmmss") + ".bak" ),
            };

            int count = ("backup database " + "Mrc" + " to disk=@BKPATH").Count();

            var dbcontext = DbContextFactory.CreateContext("");
           // dbcontext.SqlQuery("backup database " + "Mrc" + " to disk=@BKPATH");
            return Json(new { ok=""});

        }

        [HttpGet]
        public ActionResult GetClientsDataJson()
        {
            var data = new
            {
                authorizeMenu = this.GetMenus(),
            };
            return this.SuccessData(data);
        }

        [AllowNotLogin]
        public ActionResult Error()
        {
            return this.View();
        }

        List<PermissionMenu> GetMenus()
        {
            List<PermissionMenu> ret = new List<PermissionMenu>();

            IUserService userService = this.CreateService<IUserService>();
            IPermissionService authService = this.CreateService<IPermissionService>();

            

            Dictionary<string, Sys_Permission> userPermissionDic = null;
            if (!this.CurrentSession.IsAdmin)
            {
                List<Sys_Permission> userPermissions = userService.GetUserPermissions(this.CurrentSession.UserId);
                userPermissionDic = userPermissions.ToDictionary(a => a.Id);
            }

            List<Sys_Permission> permissionMenus = authService.GetPermissionMenus();

            List<Sys_Permission> parentPermissions = permissionMenus.Where(a => a.ParentId == null).ToList();

            foreach (Sys_Permission item in parentPermissions)
            {
                PermissionMenu permissionMenu = PermissionMenu.Create(item);

                List<PermissionMenu> childMenus = new List<PermissionMenu>();
                GatherChildMenus(permissionMenus, item, childMenus, userPermissionDic);

                permissionMenu.Children.AddRange(childMenus);
                ret.Add(permissionMenu);
            }

            ret = ret.Where(a => !(a.Type == PermissionType.节点组 && a.Children.Count == 0)).OrderBy(a => a.SortCode).ToList();

            return ret;
        }

        void GatherChildMenus(List<Sys_Permission> permissions, Sys_Permission permission, List<PermissionMenu> list, Dictionary<string, Sys_Permission> userPermissionDic)
        {
            var childPermissions = permissions.Where(a => a.ParentId == permission.Id).OrderBy(a => a.SortCode);
            foreach (Sys_Permission childPermission in childPermissions)
            {
                if (childPermission.Type == PermissionType.节点组)
                {
                    GatherChildMenus(permissions, childPermission, list, userPermissionDic);
                    continue;
                }

                if (childPermission.Type != PermissionType.公共菜单 && !this.CurrentSession.IsAdmin)
                {
                    if (!userPermissionDic.ContainsKey(childPermission.Id))
                        continue;
                }

                list.Add(PermissionMenu.Create(childPermission));
            }
        }
    }

    public class PermissionMenu
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public PermissionType Type { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public int SortCode { get; set; }
        public List<PermissionMenu> Children { get; set; } = new List<PermissionMenu>();

        public static PermissionMenu Create(Sys_Permission permission)
        {
            PermissionMenu ret = new PermissionMenu()
            {
                Id = permission.Id,
                Name = permission.Name,
                ParentId = permission.ParentId,
                Type = permission.Type,
                Url = permission.Url,
                Icon = permission.Icon,
                SortCode = permission.GetSortCode(),
            };

            return ret;
        }
    }
}
