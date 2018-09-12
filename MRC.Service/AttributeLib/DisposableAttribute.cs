using System;
using System.Collections.Generic;
using System.Text;

namespace MRC.Service.AttributeLib
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class DisposableAttribute : Attribute
    {
    }
}
