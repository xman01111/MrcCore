
using Chloe;
using MRC.Entity;
using MRC.Service.Application;
using MRC.Service.Models;
using MRC.ToolsAndEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRC.Domain.IService
{
    public interface IHadGetRedPacketService : IAppService
    {
        void Add(HadGetRedPacket input);
        void Update(HadGetRedPacket input);
        void Delete(string id, string operatorId);
        List<HadGetRedPacket> GetAllList();
    }
    public class HadGetRedPacketService : AppServiceBase<HadGetRedPacket>, IHadGetRedPacketService
    {
        public HadGetRedPacketService(IDbContext dbContext, IServiceProvider services) : base(dbContext, services)
        {

        }
        public void Add(HadGetRedPacket input)
        {
            this.InsertFromDto(input);
        }
        public void Update(HadGetRedPacket input)
        {
            this.UpdateFromDto(input);
        }

        public void Delete(string id, string operatorId)
        {
            this.SoftDelete(id, operatorId);
        }
        public List<HadGetRedPacket> GetAllList()
        {
            return this.Query.FilterDeleted().Where(a =>a.Id!=null).ToList();
        }
    }

}
