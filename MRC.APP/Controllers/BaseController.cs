
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MRC.Data.Enum;
using MRC.Data.Models;
using MRC.Service.AttributeLib;
using MRC.Service.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MRC.APP
{
    public abstract class BaseController : Controller
    {
        static readonly Type TypeOfCurrent = typeof(BaseController);
        static readonly Type TypeOfDisposableAttribute = typeof(DisposableAttribute);

        ILogger _logger = null;
        public ILogger Logger
        {
            get
            {
                if (this._logger == null)
                {
                    ILoggerFactory loggerFactory = this.HttpContext.RequestServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
                    ILogger logger = loggerFactory.CreateLogger(this.GetType().FullName);
                    this._logger = logger;
                }

                return this._logger;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            this.DisposeMembers();
        }
        /// <summary>
        /// 扫描对象内所有带有 DisposableAttribute 标记并实现了 IDisposable 接口的属性和字段，执行其 Dispose() 方法
        /// </summary>
        void DisposeMembers()
        {
            Type type = this.GetType();

            List<PropertyInfo> properties = new List<PropertyInfo>();
            List<FieldInfo> fields = new List<FieldInfo>();

            Type searchType = type;

            while (true)
            {
                properties.AddRange(searchType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly).Where(a =>
                {
                    if (typeof(IDisposable).IsAssignableFrom(a.PropertyType))
                    {
                        return a.CustomAttributes.Any(c => c.AttributeType == TypeOfDisposableAttribute);
                    }

                    return false;
                }));

                fields.AddRange(searchType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly).Where(a =>
                {
                    if (typeof(IDisposable).IsAssignableFrom(a.FieldType))
                    {
                        return a.CustomAttributes.Any(c => c.AttributeType == TypeOfDisposableAttribute);
                    }

                    return false;
                }));

                if (searchType == TypeOfCurrent)
                    break;
                else
                    searchType = searchType.BaseType;
            }

            foreach (var pro in properties)
            {
                IDisposable val = pro.GetValue(this) as IDisposable;
                if (val != null)
                    val.Dispose();
            }

            foreach (var field in fields)
            {
                IDisposable val = field.GetValue(this) as IDisposable;
                if (val != null)
                    val.Dispose();
            }
        }


        [NonAction]
        public ContentResult JsonContent(object obj)
        {
            string json = JsonHelper.Serialize(obj);
            return base.Content(json);
        }
        public JsonResult ReturnJson(object obj)
        {
            return base.Json(obj);
        }



        [NonAction]
        public JsonResult SuccessData(object data = null)
        {
            Result<object> result = Result.CreateResult<object>(ResultStatus.OK, data);
            return this.Json(result);
        }
        [NonAction]
        public JsonResult SuccessMsg(string msg = "操作成功")
        {
            Result result = new Result(ResultStatus.OK, msg);
            
            return this.Json(result);
        }
        [NonAction]
        public JsonResult AddSuccessData(object data, string msg = "添加成功")
        {
            Result<object> result = Result.CreateResult<object>(ResultStatus.OK, data);
            result.Msg = msg;
            return this.Json(result);
        }
        [NonAction]
        public JsonResult AddSuccessMsg(string msg = "添加成功")
        {
            return this.SuccessMsg(msg);
        }
        [NonAction]
        public JsonResult UpdateSuccessMsg(string msg = "更新成功")
        {
            return this.SuccessMsg(msg);
        }
        [NonAction]
        public JsonResult DeleteSuccessMsg(string msg = "删除成功")
        {
            return this.SuccessMsg(msg);
        }
        [NonAction]
        public JsonResult FailedMsg(string msg = null)
        {
            Result retResult = new Result(ResultStatus.Failed, msg);
            return this.Json(retResult);
        }
    }
}
