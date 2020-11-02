using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace StudentManager.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> logger;

        /// <summary>
        /// 注入ASP.NET COre ILogger服务
        /// 将控制器类型指定为泛型参数
        /// 这有助于我们确定哪个类或控制器产生了异常，然后记录它
        /// </summary>
        /// <param name="logger"></param>
        public ErrorController(ILogger<ErrorController> logger)
        {
            this.logger = logger;
        }

        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCondeHandler(int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
           
            switch (statusCode)
            {
                case 404:
                    ViewData["ErrorMessage"] = "抱歉，您访问的页面不存在";

                    logger.LogWarning($"发生了一个404错误，路径={statusCodeResult.OriginalPath},以及查询字符串={statusCodeResult.OriginalQueryString}");
                    //ViewData["Path"] = statusCodeResult.OriginalPath;
                    //ViewData["BasePath"] = statusCodeResult.OriginalPathBase;
                    //ViewData["QueryStr"] = statusCodeResult.OriginalQueryString;
                    break;
            }
            return View("NotFound");
        }

        /// <summary>
        ///  [AllowAnonymous]: 代表允许匿名返回
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("Error")]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.
        Get<IExceptionHandlerPathFeature>();

            logger.LogError($"路径:{exceptionHandlerPathFeature.Path},产生了一个错误{exceptionHandlerPathFeature.Error.Message}");
            //ViewData["ExceptionPath"] = exceptionHandlerPathFeature.Path;
            //ViewData["ExceptionMessage"] = exceptionHandlerPathFeature.Error.Message;
            //ViewData["ExceptionStackTrace"] = exceptionHandlerPathFeature.Error.StackTrace; //错误堆栈信息

            return View("Error");
        }
    }
}
