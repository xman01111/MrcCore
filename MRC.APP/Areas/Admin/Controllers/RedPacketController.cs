using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MRC.Domain;
using MRC.Domain.IService;
using MRC.Entity;
using MRC.APP;
using MRC.ToolsAndEx;
using MRC.Service.Authorization;
using MRC.Service.Helper;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Data.SqlClient;
using MRC.Data;
using MRC.Data.Models;
using MRC.Domain.Service;

namespace MRC.APP.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RedPacketController : WebBaseController
    {
        private readonly IRedPacketService _redPacketService;
        public RedPacketController(IRedPacketService RedPacketService)
        {
            this._redPacketService = RedPacketService;
        }
        [Login]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Report()
        {
            return View();
        }

        [Login]
        [HttpPost]
        public ActionResult Add(decimal Money = 0, int Num = 0, int RedPacketType = 0, int WalletType = 0)
        {
            Result result = new Result();
            if (Money == 0 || Num == 0 || RedPacketType == 0)
            {
                result.Msg = "请输入完整信息";
                return Json(result);
            }
            RedPacket redpacket = new RedPacket();
            redpacket.Id = Guid.NewGuid().ToString();
            redpacket.uuid = this.CurrentSession.UserId;
            redpacket.CreateTime = DateTime.Now;
            redpacket.EndTime = DateTime.Now.AddDays(1);
            redpacket.totalMoney = Money;
            redpacket.LessMoney = Money;
            redpacket.num = Num;
            redpacket.LessNum = Num;
            redpacket.builderStrategy = RedPacketType;
            redpacket.walletType = WalletType;
            redpacket.getuuids = "";
            redpacket.appointUuid = "";
            redpacket.remark = "";
            redpacket.token = "";
            _redPacketService.Add(redpacket);
            result.Status = Data.Enum.ResultStatus.OK;
            result.Msg = "添加成功";
            RedisHelper.Set(PrefixKeyHelper.GetRedPacketPK() + redpacket.Id, redpacket.Serialize());
            return Json(result);
        }

        [Login]
        public ActionResult GetRedPacket(string ID)
        {
            Result result = new Result();
            var redpacketStr2 = RedisHelper.Get(PrefixKeyHelper.GetRedPacketPK() + ID);
            if (redpacketStr2.IsNullOrEmpty())
            {
                result.Msg = "红包已失效";
                return Json(result);
            }
            RedPacket redpacketTest = JsonHelper.Deserialize<RedPacket>(redpacketStr2);
            RedPacketResult builderResult = new RedPacketResult();

            for (int i = 0; i < redpacketTest.num; i++)
            {
                var redpacketStr = RedisHelper.Get(PrefixKeyHelper.GetRedPacketPK() + ID);
                RedPacket redpacket = JsonHelper.Deserialize<RedPacket>(redpacketStr);
                if (redpacket.builderStrategy == (int)Data.Enum.EnumRedPacketType.FixedAverage)
                {
                    BuilderRedPacketsForEqual builder = new BuilderRedPacketsForEqual(redpacket, this.CurrentSession.UserId);
                    builderResult = builder.update();
                }
                else if (redpacket.builderStrategy == (int)Data.Enum.EnumRedPacketType.Radom)
                {
                    BuilderRedPacketsForRadom builder = new BuilderRedPacketsForRadom(redpacket, this.CurrentSession.UserId);
                    builderResult = builder.update();
                }
                else
                {

                }
            }


            if (builderResult.Status == (int)(int)Data.Enum.EnumGetRedPacketStatus.GetOK)
            {
                result.Msg = "领取成功";
                result.Status = Data.Enum.ResultStatus.OK;
            }
            else if (builderResult.Status == (int)(int)Data.Enum.EnumGetRedPacketStatus.Error)
            {
                result.Msg = builderResult.Message;
                result.Status = Data.Enum.ResultStatus.Failed;
            }
            else if (builderResult.Status == (int)(int)Data.Enum.EnumGetRedPacketStatus.Over)
            {
                result.Msg = "红包已领完";
                result.Status = Data.Enum.ResultStatus.Failed;
            }
            return Json(result);
        }

        [HttpGet]
        public ActionResult GetData()
        {
            List<List<object>> ydata = new List<List<object>>();
            List<object> xdata = new List<object>();
            int TotalMax = 0;
            int max = 1;
            var list = RedisHelper.Keys("WWHRP:*").ToList();
            foreach (var item in list)
            {
                max = 1;
                List<object> xitem = new List<object>();
                var xlistdata = RedisHelper.LRang(item, 0, -1);
                foreach (var data in xlistdata)
                {
                    HadGetRedPacket model = JsonHelper.Deserialize<HadGetRedPacket>(data);
                    xitem.Add(model.Money);
                    if (max > TotalMax)
                    {
                        xdata.Add(max);
                        TotalMax = max;
                    }
                    max++;
                }
                xitem.Reverse();
                ydata.Add(xitem);
            }
            return Json(new { xdata = xdata, ydata = ydata });
        }


        public ActionResult GetLastOne()
        {
            var LastOne = this.CreateService<IRedPacketService>().GetLast();
            Result<RedPacket> result = Result.CreateResult<RedPacket>(MRC.Data.Enum.ResultStatus.OK, LastOne);
            return Json(result);
        }

        public ActionResult GetHadList(string ID)
        {

            var data =RedisHelper.LRang(PrefixKeyHelper.GetWaitWriteHadGetRedPacket()+ID,0,-1);
            List<HadGetRedPacket> datalist = new List<HadGetRedPacket>();
            foreach (var item in data)
            {
                datalist.Add(JsonHelper.Deserialize<HadGetRedPacket>(item));
            }
            Result<List<HadGetRedPacket>> result = Result.CreateResult<List<HadGetRedPacket>>(MRC.Data.Enum.ResultStatus.OK, datalist);
            return Json(result);
        }

        public ActionResult GetRedPacket2(string ID,string uid)
        {
            Result result = new Result();
            var redpacketStr = RedisHelper.Get(PrefixKeyHelper.GetRedPacketPK() + ID);
            if (redpacketStr.IsNullOrEmpty())
            {
                result.Msg = "红包已失效";
                return Json(result);
            }
            RedPacket redpacket = JsonHelper.Deserialize<RedPacket>(redpacketStr);
            RedPacketResult builderResult = new RedPacketResult();
            if (redpacket.builderStrategy == (int)Data.Enum.EnumRedPacketType.FixedAverage)
            {
                BuilderRedPacketsForEqual builder = new BuilderRedPacketsForEqual(redpacket, uid);
                builderResult = builder.update();
            }
            else if (redpacket.builderStrategy == (int)Data.Enum.EnumRedPacketType.Radom)
            {
                BuilderRedPacketsForRadom builder = new BuilderRedPacketsForRadom(redpacket, uid);
                builderResult = builder.update();
            }
            else
            {
            }
            if (builderResult.Status == (int)(int)Data.Enum.EnumGetRedPacketStatus.GetOK)
            {
                result.Msg = "领取成功";
                result.Status = Data.Enum.ResultStatus.OK;
            }
            else if (builderResult.Status == (int)(int)Data.Enum.EnumGetRedPacketStatus.Error)
            {
                result.Msg = builderResult.Message;
                result.Status = Data.Enum.ResultStatus.Failed;
            }
            else if (builderResult.Status == (int)(int)Data.Enum.EnumGetRedPacketStatus.Over)
            {
                result.Msg = "红包已领完";
                result.Status = Data.Enum.ResultStatus.Failed;
            }
            return Json(result);
        }
    }
}