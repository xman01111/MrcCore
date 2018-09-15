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
    public interface IRedPacketService : IAppService
    {
        void Add(RedPacket input);
        void Update(RedPacket input);
        void Delete(string id, string operatorId);
        List<RedPacket> GetListById(List<string> ids);
    }
    public class RedPacketService : AppServiceBase<RedPacket>, IRedPacketService
    {
        public RedPacketService(IDbContext dbContext, IServiceProvider services) : base(dbContext, services)
        {

        }
        public void Add(RedPacket input)
        {
            this.InsertFromDto(input);
        }
        public void Update(RedPacket input)
        {
            this.UpdateFromDto(input);
        }

        public void Delete(string id, string operatorId)
        {
            this.SoftDelete(id, operatorId);
        }
        public List<RedPacket> GetListById(List<string> ids)
        {
            if (ids.Count == 0)
                return new List<RedPacket>();

            return this.Query.FilterDeleted().Where(a => ids.Contains(a.Id)).ToList();
        }
    }

}
