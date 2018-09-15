using System;
using System.Collections.Generic;
using System.Text;

namespace MRC.Data.Enum
{


    /// <summary>
    /// 钱包类型
    /// </summary>
    public enum EnumRedPacketWalletType
    {
        /// <summary>
        /// 以太坊
        /// </summary>
        ETH = 0,
        /// <summary>
        /// 比特币
        /// </summary>
        BTC = 1,
        /// <summary>
        /// EOS
        /// </summary>
        EOS = 2
    }
    /// <summary>
    /// 红包类型
    /// </summary>
    public enum EnumRedPacketType
    {
        /// <summary>
        /// 固定平分红包
        /// </summary>
        FixedAverage = 0,
        /// <summary>
        /// 随机红包
        /// </summary>
        Radom = 1,
        /// <summary>
        /// 指定红包
        /// </summary>
        Appoint = 2
    }

    /// <summary>
    /// 红包类型
    /// </summary>
    public enum EnumGetRedPacketStatus
    {
        /// <summary>
        /// 领取成功
        /// </summary>
        GetOK = 0,
        /// <summary>
        /// 已领完
        /// </summary>
        Over = 1,
        /// <summary>
        /// 领取失败
        /// </summary>
        Error =-1
    }

}
