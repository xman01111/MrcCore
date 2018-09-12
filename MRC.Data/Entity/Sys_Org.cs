using System;
using System.Collections.Generic;
using System.Text;

namespace MRC.Entity
{
    public class Sys_Org
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int? OrgType { get; set; }
        public string ManagerId { get; set; }
        public string ParentId { get; set; }
        public string Description { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletionTime { get; set; }
        public string DeleteUserId { get; set; }
    }
}
