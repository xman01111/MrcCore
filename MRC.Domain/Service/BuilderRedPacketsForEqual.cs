using MRC.Data.Models;
using MRC.Service.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using MRC.Entity;
namespace MRC.Domain.Service
{
    /// <summary>
    /// 固定平分红包类
    /// </summary>
    public class BuilderRedPacketsForEqual : IBuilderRedPackets
    {
        public RedPacket redpacket;
        public HadGetRedPacket hadGetRedPacket;
        public RedPacketResult result;
        //是否创建
        public bool isCreate = false;
        //单个红包金额
        public decimal oneMoney = 0.000M;
        //抢红包的人
        public string uuid;

        public BuilderRedPacketsForEqual(RedPacket redpacket,string uuid)
        {
            this.redpacket = redpacket;
            this.uuid = uuid;
            this.hadGetRedPacket = new HadGetRedPacket();
            this.result = new RedPacketResult();
            result.Status = (int)MRC.Data.Enum.EnumGetRedPacketStatus.Error;
        }

        public void Create()
        {
            this.hadGetRedPacket.Id = Guid.NewGuid().ToString();
            this.hadGetRedPacket.uuid = uuid;
            this.hadGetRedPacket.RedPacketID = redpacket.Id;
            this.hadGetRedPacket.CreateTime = DateTime.Now;
            this.hadGetRedPacket.Money = oneMoney;
            this.hadGetRedPacket.walletType = redpacket.walletType;
            this.hadGetRedPacket.isFinish = false;
        }

        public bool isCanBuilder()
        {
            if (this.redpacket.EndTime < DateTime.Now)
            {
                this.result.Message = "红包已过期";
                return false;
            }
            if (this.redpacket.LessNum > 0 && this.redpacket.LessMoney > 0)
            {
                return true;
            }
            string[] hadGetuuids = this.redpacket.getuuids.Split(',');
            List<String> list = new List<string>(hadGetuuids);
            if (list.Contains(uuid))
            {
                this.result.Message = "红包已被领取过";             
                return false;
            }
            else
            {
                return true;
            }
        }
        //生成红包函数  
        public void fx()
        {
            if (isCanBuilder())
            {
                //金额整除平分
                if (this.redpacket.LessNum >= 1 && this.redpacket.totalMoney % this.redpacket.num == 0)
                {
                    this.isCreate = true;
                    this.oneMoney = this.redpacket.totalMoney / this.redpacket.num;
                    this.redpacket.LessNum -= 1;
                    this.redpacket.LessMoney -= oneMoney;
                }
                //金额不整除平分
                else if (this.redpacket.totalMoney % this.redpacket.num != 0)
                {
                    if (this.redpacket.LessNum > 1)
                    {
                        this.isCreate = true;
                        this.oneMoney = this.redpacket.totalMoney / this.redpacket.num;
                        this.redpacket.LessNum -= 1;
                        this.redpacket.LessMoney -= oneMoney;
                    }
                    //剩余金额给最后一人
                    else if (this.redpacket.LessNum == 1)
                    {
                        decimal oneGet = (int)((this.redpacket.totalMoney / this.redpacket.num) * 1000M) / 100.000M;
                        this.oneMoney = redpacket.totalMoney - (oneGet * redpacket.num - 1);
                        this.isCreate = true;
                        this.redpacket.LessNum -= 1;
                        this.redpacket.LessMoney -= oneMoney;
                    }
                }
                else
                {

                }
                //记录已领的人
                if (this.isCreate==true)
                {
                    this.redpacket.getuuids += (uuid + ',');
                }
            }
        }
        //写入更新记录
        public RedPacketResult update()
        {
            fx();
            if(this.isCreate)
            {
                object o = new object();
                lock (o)
                {
                    //获取当前领取的数量
                    string Num = RedisHelper.Get(PrefixKeyHelper.GetRedPacketIncrement() + redpacket.Id);
                    if (Num == null || Num.ToInt()< redpacket.num)
                    {
                        //自增红包数
                        RedisHelper.Increment(PrefixKeyHelper.GetRedPacketIncrement() + redpacket.Id);
                        //更新红包缓存数据              
                        RedisHelper.Set(PrefixKeyHelper.GetRedPacketPK() + redpacket.Id, redpacket.Serialize());
                        //创建已领取红包记录Model
                        Create();
                        //写入待更新红包表数据库队列
                        RedisHelper.LPush(PrefixKeyHelper.GetWaitWriteRedPacket() + redpacket.Id, redpacket.Serialize());
                        //写入红包领取记录缓存队列
                        RedisHelper.LPush(PrefixKeyHelper.GetWaitWriteHadGetRedPacket() + redpacket.Id, hadGetRedPacket.Serialize());
                        result.Status = (int)MRC.Data.Enum.EnumGetRedPacketStatus.GetOK;
                    }
                    else
                    {
                        result.Status = (int)MRC.Data.Enum.EnumGetRedPacketStatus.Over;
                    }
                }
                result.Money = oneMoney;
                result.Message = "OK";
            }
            return result;
        }
    }
}
