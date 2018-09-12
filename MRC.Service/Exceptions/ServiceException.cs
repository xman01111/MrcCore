using System;
using System.Collections.Generic;
using System.Text;

namespace MRC.Service.Exceptions
{
    public class ServiceException : Exception
    {
        public ServiceException()
        {
        }
        public ServiceException(string message)
            : base(message)
        {
        }
    }
}
