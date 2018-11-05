using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WebAppCore.Ext
{
    /// <summary>
    /// 自定义 ActionResult    
    /// </summary>
    public class XmlResult : ActionResult
    {
        private readonly object _data = null;

        public XmlResult(object data)
        {
            _data = data;
        }

        public override void ExecuteResult(ActionContext context)
        {
            // ActionResult 默认都是以异步方式执行
            // 这个方法不会调用
            Console.WriteLine("not run");
            base.ExecuteResult(context);
        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            var serializer = new XmlSerializer(_data.GetType());
            var response = context.HttpContext.Response;
            response.ContentType = "application/xml";

            using (MemoryStream ms = new MemoryStream())
            {
                serializer.Serialize(ms, _data);
                byte[] res = new byte[ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(res);
                await response.Body.WriteAsync(res);
            }
        }
    }
}
