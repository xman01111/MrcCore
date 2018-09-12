using MRC.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MRC.Data
{
    public class AdminSession : MRCession<string>
    {
        public string UserId { get; set; }
        public string AccountName { get; set; }
        public string Name { get; set; }
        public string OrgIds { get; set; }
        public string RoleIds { get; set; }
        public string LoginIP { get; set; }
        public DateTime LoginTime { get; set; }
        public bool IsAdmin { get; set; }
    }
}
