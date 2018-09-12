using MRC.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MRC.Service.Application
{
    public interface IMRCSessionAppService
    {
        IMRCSession MRCSession { get; set; }
    }
}
