using Chloe;
using MRC.Data;
using MRC.Data.Enum;
using MRC.Entity;
using MRC.Service.Application;
using MRC.Service.Exceptions;
using MRC.Service.Helper;
using MRC.Service.Models;
using MRC.Service.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRC.Domain.IService
{
    public interface IAccountService : IAppService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password">经过md5加密后的密码</param>
        /// <param name="user"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool CheckLogin(string userName, string password, out Sys_User user, out string msg);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldPassword">明文</param>
        /// <param name="newPassword">明文</param>
        void ChangePassword(string userID,string oldPassword, string newPassword);
        void ModifyInfo(AccountModel input);
    }

    public class AccountAppService :AppServiceBase, IAccountService
    {


        public AccountAppService(IDbContext dbContext, IServiceProvider services) : base(dbContext, services)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="password">前端传过来的是经过md5加密后的密码</param>
        /// <param name="user"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool CheckLogin(string loginName, string password, out Sys_User user, out string msg)
        {
            user = null;
            msg = null;

            loginName.NotNullOrEmpty();
            password.NotNullOrEmpty();

            var view = this.DbContext.JoinQuery<Sys_User, Sys_UserLogOn>((u, userLogOn) => new object[]
            {
                JoinType.InnerJoin,u.Id == userLogOn.UserId
            })
            .Select((u, userLogOn) => new { User = u, UserLogOn = userLogOn });

            loginName = loginName.ToLower();
            if (CommonTool.IsMobilePhone(loginName))
            {
                view = view.Where(a => a.User.MobilePhone == loginName);
            }
            else if (CommonTool.IsEmail(loginName))
            {
                view = view.Where(a => a.User.Email == loginName);
            }
            else
            {
                view = view.Where(a => a.User.AccountName == loginName);
            }

            view = view.Where(a => a.User.State != AccountState.Closed);

            var viewEntity = view.FirstOrDefault();

            if (viewEntity == null)
            {
                msg = "账户不存在，请重新输入";
                return false;
            }
            if (!viewEntity.User.IsAdmin())
            {
                if (viewEntity.User.State == AccountState.Disabled)
                {
                    msg = "账户被禁用，请联系管理员";
                    return false;
                }
            }

            Sys_User userEntity = viewEntity.User;
            Sys_UserLogOn userLogOnEntity = viewEntity.UserLogOn;

            string dbPassword = EncryptHelper.DesEncrypt(password,KeyTool.GetEncryptKey()).ToMD5();
            if (dbPassword != userLogOnEntity.UserPassword)
            {
                msg = "密码不正确，请重新输入";
                return false;
            }

            DateTime lastVisitTime = DateTime.Now;
            this.DbContext.Update<Sys_UserLogOn>(a => a.Id == userLogOnEntity.Id, a => new Sys_UserLogOn() { LogOnCount = a.LogOnCount + 1, PreviousVisitTime = userLogOnEntity.LastVisitTime, LastVisitTime = lastVisitTime });
            user = userEntity;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldPassword">明文</param>
        /// <param name="newPassword">明文</param>
        public void ChangePassword(string userID,string oldPassword, string newPassword)
        {
           //EncryptHelper.DesEncrypt(newPassword);



            Sys_UserLogOn userLogOn = this.DbContext.Query<Sys_UserLogOn>().Where(a => a.UserId == userID).First();

            string encryptedOldPassword = EncryptHelper.DesEncrypt(oldPassword, userLogOn.UserSecretkey);

            if (encryptedOldPassword != userLogOn.UserPassword)
                throw new InvalidInputException("旧密码不正确");

            string newUserSecretkey =KeyTool.GetEncryptKey();
            string newEncryptedPassword = EncryptHelper.DesEncrypt(newPassword, newUserSecretkey);

            this.DbContext.DoWithTransaction(() =>
            {
                this.DbContext.Update<Sys_UserLogOn>(a => a.UserId == userID, a => new Sys_UserLogOn() { UserSecretkey = newUserSecretkey, UserPassword = newEncryptedPassword });
                

            });
        }

        public void ModifyInfo(AccountModel input)
        {
            if (input.AccountName.IsNotNullOrEmpty())
                input.AccountName = input.AccountName.Trim();

            input.Validate();


            Sys_User user = this.DbContext.Query<Sys_User>().FilterDeleted().Where(a => a.Id == input.UserId).AsTracking().First();

            string accountName = user.AccountName;

            if (user.AccountName.IsNullOrEmpty())
            {
                //用户名设置后不能修改
                if (input.AccountName.IsNotNullOrEmpty())
                {
                    accountName = input.AccountName.ToLower();
                   // AceUtils.EnsureAccountNameLegal(accountName);
                    bool exists = this.DbContext.Query<Sys_User>().Where(a => a.AccountName == accountName).Any();
                    if (exists)
                        throw new InvalidInputException("用户名[{0}]已存在".ToFormat(input.AccountName));
                }
            }

            user.AccountName = accountName;
            user.Name = input.Name;
            user.Gender = input.Gender;
            user.Birthday = input.Birthday;
            user.WeChat = input.WeChat;
            user.Description = input.Description;

            this.DbContext.Update<Sys_User>(user);
        }
    }

}
