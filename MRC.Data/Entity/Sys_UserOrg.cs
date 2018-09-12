using System;
using System.Collections.Generic;
using System.Text;

namespace MRC.Entity
{
    public class Sys_UserOrg
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string OrgId { get; set; }
        public bool DisablePermission { get; set; }
        public Sys_Org Org { get; set; }
    }
}
