
using MRC.AutoMapper;
using MRC.Data;
using MRC.Entity;
using MRC.ToolsAndEx.Extensions;

namespace MRC.Data
{
    public class AddOrUpdatePermissionInputBase : ValidationModel
    {
        public string Name { get; set; }
        /// <summary>
        /// 权限码
        /// </summary>
        public string Code { get; set; }
        public string ParentId { get; set; }
        public PermissionType Type { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public int? SortCode { get; set; }
    }
    [MapToType(typeof(Sys_Permission))]
    public class AddPermissionInput : AddOrUpdatePermissionInputBase
    {
    }
    [MapToType(typeof(Sys_Permission))]
    public class UpdatePermissionInput : AddOrUpdatePermissionInputBase
    {
        public string Id { get; set; }
        public override void Validate()
        {
            base.Validate();
            if (this.ParentId == this.Id)
                throw new InvalidInputException("上级节点不能为节点自身");
        }
    }
}
