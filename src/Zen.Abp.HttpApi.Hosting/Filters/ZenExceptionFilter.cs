using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.AspNetCore.Mvc.ExceptionHandling;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ExceptionHandling;
using Volo.Abp.Http;
using Volo.Abp.Json;
using Zen.Abp.Domain.Shared;

// ReSharper disable once CheckNamespace
namespace Zen.Abp.HttpApi.Hosting
{
    public class ZenExceptionFilter : IAsyncExceptionFilter, ITransientDependency
    {
        private readonly IExceptionToErrorInfoConverter _errorInfoConverter;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IHttpExceptionStatusCodeFinder _statusCodeFinder;

        public ZenExceptionFilter(
            IExceptionToErrorInfoConverter errorInfoConverter,
            IHttpExceptionStatusCodeFinder statusCodeFinder,
            IJsonSerializer jsonSerializer)
        {
            _errorInfoConverter = errorInfoConverter;
            _statusCodeFinder = statusCodeFinder;
            _jsonSerializer = jsonSerializer;
            Logger = NullLogger<AbpExceptionFilter>.Instance;
        }

        public ILogger<AbpExceptionFilter> Logger { get; set; }

        public async Task OnExceptionAsync(ExceptionContext context)
        {
            if (!ShouldHandleException(context))
            {
                return;
            }

            await HandleAndWrapException(context);
        }

        protected virtual bool ShouldHandleException(ExceptionContext context)
        {
            //TODO: Create DontWrap attribute to control wrapping..?

            if (context.ActionDescriptor.IsControllerAction() &&
                context.ActionDescriptor.HasObjectResult())
            {
                return true;
            }

            if (context.HttpContext.Request.CanAccept(MimeTypes.Application.Json))
            {
                return true;
            }

            if (context.HttpContext.Request.IsAjax())
            {
                return true;
            }

            return false;
        }

        protected virtual async Task HandleAndWrapException(ExceptionContext context)
        {
            //TODO: Trigger an AbpExceptionHandled event or something like that.

            context.HttpContext.Response.Headers.Add(AbpHttpConsts.AbpErrorFormat, "true");
            context.HttpContext.Response.StatusCode =
                (int) _statusCodeFinder.GetStatusCode(context.HttpContext, context.Exception);

            var remoteServiceErrorInfo = _errorInfoConverter.Convert(context.Exception);

            var exceptionApiResult = WrapExceptionApiResult(context.Exception, remoteServiceErrorInfo.Code);
            context.Result = new ObjectResult(exceptionApiResult);

            var logLevel = context.Exception.GetLogLevel();

            Logger.LogWithLevel(logLevel, $"---------- {nameof(RemoteServiceErrorInfo)} ----------");
            Logger.LogWithLevel(logLevel, _jsonSerializer.Serialize(remoteServiceErrorInfo, indented: true));
            Logger.LogException(context.Exception, logLevel);

            await context.HttpContext
                .RequestServices
                .GetRequiredService<IExceptionNotifier>()
                .NotifyAsync(
                    new ExceptionNotificationContext(context.Exception)
                );

            context.Exception = null; //Handled!避免被中间件再次处理
        }

        private ApiResult<string> WrapExceptionApiResult(Exception ex, string exceptionCode)
        {
            int.TryParse(exceptionCode, out var exCode);

            if (!Enum.IsDefined(typeof(ApiResultCode), exCode))
            {
                exCode = (int) ApiResultCode.Failed;
            }

            var result = new ApiResult<string>
            {
                Code = exCode,
                Message = ex.Message,
                Data = string.Empty
            };
            return result;
        }
    }
}