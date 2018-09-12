
using MRC.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MRC.Service.Application
{
    public class AppServiceFactory : IAppServiceFactory
    {
        IServiceProvider _serviceProvider = null;
        public IMRCSession Session { get; set; }

        public AppServiceFactory(IServiceProvider serviceProvider)
            : this(serviceProvider, null)
        {
        }
        public AppServiceFactory(IServiceProvider serviceProvider, IMRCSession session)
        {
            this._serviceProvider = serviceProvider;
            this.Session = session;
        }

       

        public virtual T CreateService<T>()
        {
            object serviceObj = this._serviceProvider.GetService(typeof(T));
            if (serviceObj == null)
                throw new Exception("Can not find the service.");

            T service = (T)serviceObj;

            IAppServiceFactoryProvider factoryProvider = service as IAppServiceFactoryProvider;
            if (factoryProvider != null)
            {
                factoryProvider.ServiceFactory = this;
            }

            if (service is IMRCSessionAppService)
            {
                ((IMRCSessionAppService)service).MRCSession = this.Session;
            }

            return service;
        }

        public void Dispose()
        {

        }
    }
}