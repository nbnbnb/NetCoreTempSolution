using System;
using System.Collections.Generic;
using System.Text;
using WebApiClient;
using WebApiClient.Attributes;

namespace ConsoleAppCoreTest.Concrete
{
    [HttpHost("https://www.baidu.com/")]
    public interface IMyWebApi : IHttpApi
    {
        [HttpGet]
        ITask<string> GetContent();
    }
}
