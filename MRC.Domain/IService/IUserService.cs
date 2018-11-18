using Chloe;
using Mrc.Data;
using MRC.Data;
using MRC.Data.Enum;
using MRC.Entity;
using MRC.Service.Application;
using MRC.Service.Exceptions;
using MRC.Service.Helper;
using MRC.Service.Models;
using MRC.Service.Tools;
using MRC.ToolsAndEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRC.Domain.IService
{
    public interface IUserService : IAppService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pwdText">明文</param>
        void RevisePassword(string userId, string pwdText);

        void Add(AddUserInput input);
        void Update(UpdateUserInput input);
        void ChangeState(string id, AccountState newState);
        void ChangeUserOrgPermissionState(string userId, string orgId, bool newState);

        List<Sys_UserPermission> GetPermissions(string id);
        void SetPermission(string id, List<string> permissionList);
        PagedData<Sys_User> GetPageData(Pagination page, string keyword);

        List<Sys_Permission> GetUserPermissions(string id);
        List<string> GetUserPermits(string id);
    }

    public class UserService : AppServiceBase<Sys_User>, IUserService
    {
        public UserService(IDbContext dbContext, IServiceProvider services) : base(dbContext, services)
        {
        
        }
        public void RevisePassword(string userId, string pwdText)
        {
            userId.NotNullOrEmpty("用户 Id 不能为空");

            var user = this.DbContext.QueryByKey<Sys_User>(userId);
            if (user == null)
                throw new InvalidInputException("用户不存在");
            if (user.State == AccountState.Closed)
                throw new InvalidInputException("无法修改已注销用户");

             user.EnsureIsNotAdmin();

            //string userSecretkey = UserHelper.GenUserSecretkey();
            //string encryptedPassword = PasswordHelper.Encrypt(pwdText, userSecretkey);

            //this.DbContext.DoWithTransaction(() =>
            //{
            //    this.DbContext.Update<Sys_UserLogOn>(a => a.UserId == userId, a => new Sys_UserLogOn() { UserSecretkey = userSecretkey, UserPassword = encryptedPassword });
            //    this.Log(LogType.Update, "User", true, "重置用户[{0}]密码".ToFormat(userId));
            //});
        }

        public void Add(AddUserInput input)
        {
            this.Trim(input);

            input.Validate();

            if (input.AccountName.IsNullOrEmpty() && input.MobilePhone.IsNullOrEmpty() && input.Email.IsNullOrEmpty())
            {
                throw new InvalidInputException("用户名/手机号码/邮箱至少填一个");
            }

            string accountName = null;
            if (input.AccountName.IsNotNullOrEmpty())
            {
                accountName = input.AccountName.ToLower();
                CommonTool.EnsureAccountNameLegal(accountName);
                bool exists = this.DbContext.Query<Sys_User>().Where(a => a.AccountName == accountName).Any();
                if (exists)
                    throw new InvalidInputException("用户名[{0}]已存在".ToFormat(input.AccountName));
            }

            string mobilePhone = null;
            if (input.MobilePhone.IsNotNullOrEmpty())
            {
                mobilePhone = input.MobilePhone;
                if (CommonTool.IsMobilePhone(mobilePhone) == false)
                    throw new InvalidInputException("请输入正确的手机号码");

                bool exists = this.DbContext.Query<Sys_User>().Where(a => a.MobilePhone == mobilePhone).Any();
                if (exists)
                    throw new InvalidInputException("手机号码[{0}]已存在".ToFormat(mobilePhone));
            }

            string email = null;
            if (input.Email.IsNotNullOrEmpty())
            {
                email = input.Email.ToLower();
                if (CommonTool.IsEmail(email) == false)
                    throw new InvalidInputException("请输入正确的邮箱地址");

                bool exists = this.DbContext.Query<Sys_User>().Where(a => a.Email == email).Any();
                if (exists)
                    throw new InvalidInputException("邮箱地址[{0}]已存在".ToFormat(input.Email));
            }

            Sys_User user =new Sys_User();
            user.AccountName = accountName;
            user.Name = input.Name;
            user.Gender = input.Gender;
            user.MobilePhone = mobilePhone;
            user.Birthday = input.Birthday;
            user.WeChat = input.WeChat;
            user.Email = email;
            user.Description = input.Description;
            user.State = AccountState.Normal;

            string userSecretkey = KeyTool.GetEncryptKey();
            string encryptedPassword = EncryptHelper.DesEncrypt(input.Password, userSecretkey);

            Sys_UserLogOn logOnEntity = new Sys_UserLogOn();
            logOnEntity.Id = IdHelper.CreateStringSnowflakeId();
            logOnEntity.UserId = user.Id;
            logOnEntity.UserSecretkey = userSecretkey;
            logOnEntity.UserPassword = encryptedPassword;

            List<string> roleIds = input.GetRoles();
            List<Sys_UserRole> userRoles = roleIds.Select(a =>
             {
                 return new Sys_UserRole()
                 {
                     Id = IdHelper.CreateStringSnowflakeId(),
                     UserId = user.Id,
                     RoleId = a,
                 };
             }).ToList();

            user.RoleIds = string.Join(",", roleIds);

            List<string> orgIds = input.GetOrgs();
            List<Sys_UserOrg> userOrgs = orgIds.Select(a =>
            {
                return new Sys_UserOrg()
                {
                    Id = IdHelper.CreateStringSnowflakeId(),
                    UserId = user.Id,
                    OrgId = a,
                    DisablePermission = false
                };
            }).ToList();

            user.OrgIds = string.Join(",", orgIds);

            List<string> postIds = input.GetPosts();
            List<Sys_UserPost> userPosts = postIds.Select(a =>
            {
                return new Sys_UserPost()
                {
                    Id = IdHelper.CreateStringSnowflakeId(),
                    UserId = user.Id,
                    PostId = a
                };
            }).ToList();

            user.PostIds = string.Join(",", postIds);

            this.DbContext.DoWithTransaction(() =>
            {
                this.DbContext.Insert(user);
                this.DbContext.Insert(logOnEntity);
                this.DbContext.InsertRange(userRoles);
                this.DbContext.InsertRange(userOrgs);
                this.DbContext.InsertRange(userPosts);
            });
        }
        public void Update(UpdateUserInput input)
        {
            this.Trim(input);

            input.Validate();

            Sys_User user = this.Query.Where(a => a.Id == input.Id).AsTracking().First();

            user.EnsureIsNotAdmin();
            if (user.State == AccountState.Closed)
                throw new InvalidInputException("无法修改已注销用户");

            string accountName = null;
            if (user.AccountName.IsNullOrEmpty())
            {
                //用户名设置后不能修改
                if (input.AccountName.IsNotNullOrEmpty())
                {
                    accountName = input.AccountName.ToLower();

                    bool exists = this.DbContext.Query<Sys_User>().Where(a => a.AccountName == accountName).Any();
                    if (exists)
                        throw new InvalidInputException("用户名[{0}]已存在".ToFormat(input.AccountName));
                }
            }
            else
                accountName = user.AccountName;

            string mobilePhone = null;
            if (user.MobilePhone.IsNotNullOrEmpty() && input.MobilePhone.IsNullOrEmpty())
            {
                //手机号码设置后不能再改为空
                throw new InvalidInputException("请输入手机号码");
            }
            if (input.MobilePhone.IsNotNullOrEmpty())
            {
                mobilePhone = input.MobilePhone;
                //if (AceUtils.IsMobilePhone(mobilePhone) == false)
                //    throw new InvalidInputException("请输入正确的手机号码");

                if (user.MobilePhone != mobilePhone)//不等说明手机号码有变
                {
                    bool exists = this.DbContext.Query<Sys_User>().Where(a => a.MobilePhone == mobilePhone).Any();
                    if (exists)
                        throw new InvalidInputException("手机号码[{0}]已存在".ToFormat(mobilePhone));
                }
            }

            string email = null;
            
            if (user.Email.IsNotNullOrEmpty() && input.Email.IsNullOrEmpty())
            {
                //邮箱地址设置后不能再改为空
                throw new InvalidInputException("请输入邮箱地址");
            }
            if (input.Email.IsNotNullOrEmpty())
            {
                email = input.Email.ToLower();
                //if (AceUtils.IsEmail(email) == false)
                //    throw new InvalidInputException("请输入正确的邮箱地址");

                if (user.Email != email)//不等说明邮箱有变
                {
                    bool exists = this.DbContext.Query<Sys_User>().Where(a => a.Email == email).Any();
                    if (exists)
                        throw new InvalidInputException("邮箱地址[{0}]已存在".ToFormat(input.Email));
                }
            }

            user.AccountName = accountName;
            user.Name = input.Name;
            user.Gender = input.Gender;
            user.MobilePhone = mobilePhone;
            user.Birthday = input.Birthday;
            user.WeChat = input.WeChat;
            user.Email = email;
            user.Description = input.Description;

            List<string> roleIds = input.GetRoles();
            List<Sys_UserRole> userRoles = this.DbContext.Query<Sys_UserRole>().Where(a => a.UserId == input.Id).ToList();
            List<string> userRolesToDelete = userRoles.Where(a => !roleIds.Contains(a.Id)).Select(a => a.Id).ToList();
            List<Sys_UserRole> userRolesToAdd = roleIds.Where(a => !userRoles.Any(r => r.Id == a)).Select(a =>
            {
                return new Sys_UserRole()
                {
                    Id = IdHelper.CreateStringSnowflakeId(),
                    UserId = input.Id,
                    RoleId = a,
                };
            }).ToList();

            user.RoleIds = string.Join(",", roleIds);

            List<string> orgIds = input.GetOrgs();
            List<Sys_UserOrg> userOrgs = this.DbContext.Query<Sys_UserOrg>().Where(a => a.UserId == input.Id).ToList();
            List<string> userOrgsToDelete = userOrgs.Where(a => !orgIds.Contains(a.Id)).Select(a => a.Id).ToList();
            List<Sys_UserOrg> userOrgsToAdd = orgIds.Where(a => !userOrgs.Any(r => r.Id == a)).Select(a =>
            {
                return new Sys_UserOrg()
                {
                    Id = IdHelper.CreateStringSnowflakeId(),
                    UserId = input.Id,
                    OrgId = a,
                    DisablePermission = false
                };
            }).ToList();

            user.OrgIds = string.Join(",", orgIds);

            List<string> postIds = input.GetPosts();
            List<Sys_UserPost> userPosts = postIds.Select(a =>
            {
                return new Sys_UserPost()
                {
                    Id = IdHelper.CreateStringSnowflakeId(),
                    UserId = input.Id,
                    PostId = a
                };
            }).ToList();

            user.PostIds = string.Join(",", postIds);

            this.DbContext.DoWithTransaction(() =>
            {
                this.DbContext.Delete<Sys_UserRole>(a => a.Id.In(userRolesToDelete));
                this.DbContext.InsertRange(userRolesToAdd);

                this.DbContext.Delete<Sys_UserOrg>(a => a.Id.In(userOrgsToDelete));
                this.DbContext.InsertRange(userOrgsToAdd);

                this.DbContext.Delete<Sys_UserPost>(a => a.UserId == input.Id);
                this.DbContext.InsertRange(userPosts);

                this.DbContext.Update<Sys_User>(user);
            });
        }
        void Trim(UserModelBase input)
        {
            if (input.AccountName.IsNotNullOrEmpty())
                input.AccountName = input.AccountName.Trim();
            if (input.MobilePhone.IsNotNullOrEmpty())
                input.MobilePhone = input.MobilePhone.Trim();
            if (input.Email.IsNotNullOrEmpty())
                input.Email = input.Email.Trim();
        }

        public void ChangeState(string id, AccountState newState)
        {
            id.NotNullOrEmpty();
            Sys_User user = this.Query.Where(a => a.Id == id).AsTracking().FirstOrDefault();
            user.EnsureIsNotAdmin();
            if (user == null)
                throw new InvalidInputException("用户不存在");
            if (user.State == AccountState.Closed)
                throw new InvalidInputException("用户已注销，无法操作");

            List<AccountState> list = new List<AccountState>() { AccountState.Normal, AccountState.Disabled, AccountState.Closed };
            if (newState.In(list) == false)
                throw new InvalidInputException("状态无效：" + newState.ToString());

            user.State = newState;

            this.DbContext.DoWithTransaction(() =>
            {
                this.DbContext.Update(user);
               // this.Log(LogType.Update, "User", true, $"用户[{this.Session.UserId}]修改用户[{id}]状态为：{newState}");
            });
        }
        public void ChangeUserOrgPermissionState(string userId, string orgId, bool newState)
        {
            userId.NotNullOrEmpty();
            orgId.NotNullOrEmpty();

            this.DbContext.Update<Sys_UserOrg>(a => a.UserId == userId && a.OrgId == orgId, a => new Sys_UserOrg() { DisablePermission = newState });
        }

        public List<Sys_UserPermission> GetPermissions(string id)
        {
            return this.DbContext.Query<Sys_UserPermission>().Where(t => t.UserId == id).ToList();
        }
        public void SetPermission(string id, List<string> permissionList)
        {
            id.NotNullOrEmpty();

            List<Sys_UserPermission> rolePermissions = permissionList.Select(a => new Sys_UserPermission()
            {
                Id = IdHelper.CreateStringSnowflakeId(),
                UserId = id,
                PermissionId = a
            }).ToList();

            this.DbContext.DoWithTransaction(() =>
            {
                this.DbContext.Delete<Sys_UserPermission>(a => a.UserId == id);
                this.DbContext.InsertRange(rolePermissions);
            });
        }
        public PagedData<Sys_User> GetPageData(Pagination page, string keyword)
        {
            var q = this.DbContext.Query<Sys_User>();

            q = q.WhereIfNotNullOrEmpty(keyword, a => a.AccountName.Contains(keyword) || a.Name.Contains(keyword) || a.MobilePhone.Contains(keyword));
            q = q.Where(a => a.AccountName != keyword);
            q = q.OrderByDesc(a => a.CreationTime);

            PagedData<Sys_User> pagedData = q.TakePageData(page);

            List<string> userIds = pagedData.Models.Select(a => a.Id).ToList();
            List<string> postIds = pagedData.Models.SelectMany(a => a.PostIds.SplitString()).Distinct().ToList();
            List<string> roleIds = pagedData.Models.SelectMany(a => a.RoleIds.SplitString()).Distinct().ToList();

            List<Sys_Post> posts = this.DbContext.Query<Sys_Post>().Where(a => a.Id.In(postIds)).ToList();
            List<Sys_Role> roles = this.DbContext.Query<Sys_Role>().Where(a => a.Id.In(roleIds)).ToList();

            List<Sys_UserOrg> userOrgs = this.DbContext.Query<Sys_UserOrg>().InnerJoin<Sys_Org>((a, b) => a.OrgId == b.Id)
                    .Where((a, b) => userIds.Contains(a.UserId))
                    .Select((a, b) => new Sys_UserOrg() { Id = a.Id, UserId = a.UserId, OrgId = a.OrgId, DisablePermission = a.DisablePermission, Org = b }).ToList();

            foreach (Sys_User user in pagedData.Models)
            {
                user.UserOrgs.AddRange(userOrgs.Where(a => a.UserId == user.Id));

                List<string> userPostIds = user.PostIds.SplitString();
                user.Posts.AddRange(posts.Where(a => a.Id.In(userPostIds)));

                List<string> userRoleIds = user.RoleIds.SplitString();
                user.Roles.AddRange(roles.Where(a => a.Id.In(userRoleIds)));
            }

            return pagedData;
        }

        public List<Sys_Permission> GetUserPermissions(string id)
        {
            List<Sys_Permission> ret = new List<Sys_Permission>();

            List<string> userPermissionIds = this.DbContext.Query<Sys_UserPermission>().Where(a => a.UserId == id).Select(a => a.PermissionId).ToList();

            List<string> rolePermissionIds = this.DbContext.JoinQuery<Sys_RolePermission, Sys_Role, Sys_UserRole>((rolePermission, role, userRole) => new object[] {
                JoinType.InnerJoin,rolePermission.RoleId==role.Id,
                JoinType.InnerJoin,role.Id==userRole.RoleId
            })
            .Where((rolePermission, role, userRole) => userRole.UserId == id)
            .Select((rolePermission, role, userRole) => rolePermission.PermissionId).ToList();

            List<string> orgPermissionIds = this.DbContext.JoinQuery<Sys_OrgPermission, Sys_Org, Sys_UserOrg>((orgPermission, org, userOrg) => new object[] {
                JoinType.InnerJoin,orgPermission.OrgId==org.Id,
                JoinType.InnerJoin,org.Id==userOrg.OrgId
            })
            .Where((orgPermission, org, userOrg) => userOrg.UserId == id && userOrg.DisablePermission == false)
            .Select((orgPermission, org, userOrg) => orgPermission.PermissionId).ToList();

            List<string> permissionIds = userPermissionIds.Concat(rolePermissionIds).Concat(orgPermissionIds).Distinct().ToList();

            ret = this.DbContext.Query<Sys_Permission>().Where(a => permissionIds.Contains(a.Id)).ToList();

            return ret;
        }
        public List<string> GetUserPermits(string id)
        {
            var ret = this.GetUserPermissions(id).Where(a => a.Code.IsNotNullOrEmpty()).Select(a => a.Code).ToList();
            return ret;
        }
    }
}
