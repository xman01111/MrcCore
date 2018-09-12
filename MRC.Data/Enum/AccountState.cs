using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRC.Data.Enum
{
    public enum AccountState
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 1,
        /// <summary>
        /// 被禁用
        /// </summary>
        Disabled = 2,
        /// <summary>
        /// 已注销
        /// </summary>
        Closed = 3
    }
}