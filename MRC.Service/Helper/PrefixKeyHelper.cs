using System;
using System.Collections.Generic;
using System.Text;

namespace MRC.Service.Helper
{
    public static class PrefixKeyHelper
    {
        /// <summary>
        /// 红包表前缀
        /// </summary>
        private static  readonly string RedPacketPK= "RP:";

        /// <summary>
        /// 红包领取自增计数
        /// </summary>
        private static readonly string RedPacketIncrement = "RP:Ict:";

        /// <summary>
        /// 已领红包前缀
        /// </summary>
        private static readonly string HadGetRedPacketPK = "HRP:";

        /// <summary>
        /// 已领红包待处理队列
        /// </summary>
        private static readonly string WaitWriteHadGetRedPacket = "WWHRP:";

        /// <summary>
        /// 红包表更新待处理队列
        /// </summary>
        private static readonly string WaitWriteRedPacket = "WWRP:";

        public static string GetRedPacketPK()
        {
            return RedPacketPK;
        }
        public static string GetRedPacketIncrement()
        {
            return RedPacketIncrement;
        }

        public static string GetHadGetRedPacketPK()
        {
            return HadGetRedPacketPK;
        }

        public static string GetWaitWriteHadGetRedPacket()
        {
            return WaitWriteHadGetRedPacket;
        }

        public static string GetWaitWriteRedPacket()
        {
            return WaitWriteRedPacket;
        }
    }
}
