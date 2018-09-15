using System;
using System.Collections.Generic;
using System.Text;

namespace MRC.Data.Models
{
    public class RedPacketDto
    {       
        /// <summary>
        /// 红包总金额
        /// </summary>
        public decimal totalMoney { get; set; }
        /// <summary>
        /// 红包数量
        /// </summary>
        public int num { get; set; }
        
        /// <summary>
        /// 范围开始
        /// </summary>
        public decimal rangeStart { get; set; }

        /// <summary>
        /// 范围结算
        /// </summary>
        public decimal rangeEnd { get; set; }

        /// <summary>
        /// 生成红包策略
        /// </summary>
        public int builderStrategy { get; set; }

        //随机红包剩余规则  
        public int randFormatType { get; set; }

        public RedPacketDto Create
        (
            decimal totalMoney,
            int num,
            decimal rangeStart,
            decimal rangeEnd,
            int builderStrategy,
            int randFormatType
        )
        {
            RedPacketDto ret = new RedPacketDto();
            ret.totalMoney = totalMoney;
            ret.num = num;
            ret.rangeStart = rangeStart;
            ret.rangeEnd = rangeEnd;
            ret.builderStrategy = builderStrategy;
            ret.randFormatType = randFormatType;
            return ret;
        }
    }





}