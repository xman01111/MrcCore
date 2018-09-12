using System;
using System.Collections.Generic;
using System.Text;

namespace MRC.Data
{
    public interface IMRCSession
    {
    }
    public class MRCession<T> : IMRCSession
    {
        public T UserId { get; set; }
    }
}
