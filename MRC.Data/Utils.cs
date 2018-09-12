using System;
using System.Collections.Generic;
using System.Text;

namespace MRC.Data
{
    static class Utils
    {
        public static void CheckNull(object obj, string paramName = null)
        {
            if (obj == null)
                throw new ArgumentNullException(paramName);
        }
    }
}
