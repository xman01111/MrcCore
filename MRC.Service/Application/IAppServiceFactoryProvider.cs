﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MRC.Service.Application
{
    public interface IAppServiceFactoryProvider
    {
        IAppServiceFactory ServiceFactory { get; set; }
    }
}
