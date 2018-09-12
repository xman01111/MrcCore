using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MRC.APP.Models
{
    public interface ITestPropertyInfo
    {
        Type GetThisType<T>();
    }

    public class PropertyInfoServer: ITestPropertyInfo
    {

        public Type GetThisType<T>()
        {
            return typeof(T);
        }
    }
}
