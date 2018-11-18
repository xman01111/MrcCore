using MRC.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MRC.Service.Application;
using MRC.Data.Enum;

namespace MRC.Service.Models
{
    public class UserModelBase : ValidationModel
    {
        public string AccountName { get; set; }
        public string Name { get; set; }
        public Gender? Gender { get; set; }
        public string MobilePhone { get; set; }
        public DateTime? Birthday { get; set; }
        public string WeChat { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }

        public string Roles { get; set; }
        public string Orgs { get; set; }
        public string Posts { get; set; }

        public string CreateUserId { get; set; }
        

        public List<string> GetRoles()
        {
            return this.Roles.SplitString(',');
        }
        public List<string> GetOrgs()
        {
            return this.Orgs.SplitString(',');
        }
        public List<string> GetPosts()
        {
            return this.Posts.SplitString(',');
        }
    }

    public class AddUserInput : UserModelBase
    {
        /// <summary>
        /// 创建用户时，传的是明文
        /// </summary>
        [RequiredAttribute(ErrorMessage = "密码不能为空")]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "密码必须是{2}-{1}位")]
        public string Password { get; set; }
    }

    public class UpdateUserInput : UserModelBase
    {
        [RequiredAttribute(ErrorMessage = "{0}不能为空")]
        public string Id { get; set; }
    }
}
