using System;
using System.Collections.Generic;
using System.Text;
using MRC.Data;
namespace MRC.Service.Tools
{
    public static class KeyTool
    {
        public static string GetEncryptKey()
        {
           return Globals.Configuration["Key:EncryptKey"];
        }
    }
}
