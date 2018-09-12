using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRC.Service.Application
{
    public interface IAppServiceFactory : IDisposable
    {
        T CreateService<T>();
    }
}
