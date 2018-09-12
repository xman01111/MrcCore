using System;
using System.Collections.Generic;
using System.Text;

namespace MRC.Entity
{
    public class Sys_Post
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string OrgId { get; set; }
        public string Description { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletionTime { get; set; }
    }
}
