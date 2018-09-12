using System;
using System.Collections.Generic;
using System.Text;

namespace MRC.Data.Enum
{
    public enum ResultStatus
    {
        OK = 100,
        Failed = 101,
        /// <summary>
        /// 表示未登录
        /// </summary>
        NotLogin = 102,
        /// <summary>
        /// 表示未授权
        /// </summary>
        Unauthorized = 103,
    }
}
