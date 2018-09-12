using System;
using Microsoft.Extensions.Configuration;
namespace MRC.Data
{
    public static class Globals
    {
        public static IServiceProvider Services { get; set; }
        public static IConfigurationRoot Configuration { get; set; }
    }
}
