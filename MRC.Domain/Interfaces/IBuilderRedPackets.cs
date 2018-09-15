using MRC.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using MRC.Entity;
namespace MRC.Domain
{
    public interface IBuilderRedPackets
    {
        //创建红包  
        void Create();
        //是否可以生成红包  
        bool isCanBuilder();
        //生成红包函数  
        void fx();
        //写入更新记录
        RedPacketResult update();
    }
}
