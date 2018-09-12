using Chloe.Entity;
using MRC.Data.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRC.Entity
{
    public class Sys_Log
    {
        [Sequence("Sys_Log_AUTOID")]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public LogType Type { get; set; }
        public string IP { get; set; }
        public string IPAddress { get; set; }
        public string ModuleId { get; set; }
        public string ModuleName { get; set; }
        public bool? Result { get; set; }
        public string Description { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
