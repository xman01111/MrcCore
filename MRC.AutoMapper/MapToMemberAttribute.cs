using System;
using System.Collections.Generic;
using System.Text;

namespace MRC.AutoMapper
{
    /// <summary>
    /// 配合 MapToTypeAttribute 使用
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MapToMemberAttribute : Attribute
    {
        public MapToMemberAttribute(string memberName)
        {
            this.MemberName = memberName;
        }
        public string MemberName { get; set; }
    }
}
