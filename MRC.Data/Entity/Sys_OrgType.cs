using System;
using System.Collections.Generic;
using System.Text;

namespace MRC.Entity
{
    public class Sys_OrgType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
    }
}
