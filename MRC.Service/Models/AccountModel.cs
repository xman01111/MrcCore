using MRC.Data.Enum;
using MRC.Service.Application;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MRC.Service.Models
{
    public class AccountModel : ValidationModel
    {
        public string UserId { get; set; }
        public string AccountName { get; set; }
        [Required(ErrorMessage = "姓名不能为空")]
        public string Name { get; set; }
        public Gender? Gender { get; set; }
        public DateTime? Birthday { get; set; }

        public string WeChat { get; set; }
        public string Description { get; set; }
    }
}
