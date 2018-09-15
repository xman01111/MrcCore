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
        private  readonly IRedPacketService _redPacketService;
        public RedPacketController(IRedPacketService RedPacketService)
        {
            this._redPacketService = RedPacketService;
        }
        [Login]
        public IActionResult Index()
        {
            return View();
        }

        [Login]
        [HttpPost]
        public ActionResult Add(decimal Money=0,int Num=0,int RedPacketType=0,int WalletType=0)
        {
            Result result = new Result();
            if (Money==0|| Num==0|| RedPacketType==0)
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
            var redpacketStr=RedisHelper.Get(PrefixKeyHelper.GetRedPacketPK() + ID);
            if (redpacketStr.IsNullOrEmpty())
            {
                result.Msg = "红包已失效";
                return Json(result);
            }
            RedPacket redpacket=JsonHelper.Deserialize<RedPacket>(redpacketStr);
            RedPacketResult builderResult = new RedPacketResult();
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
            List<object> xdata = new List<object>();
            List<object> ydata = new List<object>();
            int max=1;
            var listdata111 = RedisHelper.LPop("WWHRP:dfa4e36d-daed-4c9b-820f-7a0933dfc2b1");
            var list= RedisHelper.Keys("keyWWHRP:*").ToList();
            foreach (var item in list)
            {
              var len = RedisHelper.LLen(item);
              var listdata=RedisHelper.LRang(item,0, 7);
              
            }

            return Json(list);
        }

    }
}