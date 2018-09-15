using System;
using System.Collections.Generic;
using System.Text;

namespace MRC.Entity
{
    // 红包领取表
    public class HadGetRedPacket
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
        ///红包表id
        /// </summary>
        public string RedPacketID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 红包金额
        /// </summary>
        public decimal Money { get; set; }

        /// <summary>
        /// 红包币种
        /// </summary>
        public int walletType { get; set; }

        /// <summary>
        /// 已完成（结算进钱包）
        /// </summary>
        public bool isFinish { get; set; }
    }
}
