using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MRC.APP.Models
{
    public class TestPropertyInfo2: PropertyInfoServer
    {     
        public string Name { get; set; }
        public int ID { get; set; }       
        public int Card { get; set; }

        public Type Get()
        {
           return this.GetThisType<TestPropertyInfo2>();
        }
    }
}
