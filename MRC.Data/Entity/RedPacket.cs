using System;
using System.Collections.Generic;
using System.Text;

namespace MRC.Entity
{

    /// <summary>
    /// 红包表
    /// </summary>
    public class RedPacket
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///用户id
        /// </summary>
        public string uuid { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 红包总金额
        /// </summary>
        public decimal totalMoney { get; set; }

        /// <summary>
        /// 剩余金额
        /// </summary>
        public decimal LessMoney { get; set; }

        /// <summary>
        /// 红包数量
        /// </summary>
        public int num { get; set; }

        /// <summary>
        /// 剩余红包数
        /// </summary>
        public int LessNum { get; set; }

        /// <summary>
        /// 红包类型（固定红包 随机红包 指定用户红包）
        /// </summary>
        public int builderStrategy { get; set; }

        /// <summary>
        /// 红包币种
        /// </summary>
        public int walletType { get; set; }

        /// <summary>
        /// 已抢红包的人ID
        /// </summary>
        public string getuuids { get; set; }
        /// <summary>
        /// 指定人ID
        /// </summary>
        public string appointUuid { get; set; }

        /// <summary>
        /// 红包信息
        /// </summary>
        public string remark { get; set; }

        /// <summary>
        /// 红包口令
        /// </summary>
        public string token { get; set; }

    }
}
