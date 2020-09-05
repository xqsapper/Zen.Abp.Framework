using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Volo.Abp.Aspects;
using Volo.Abp.Auditing;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Users;

// ReSharper disable once CheckNamespace
namespace Zen.Abp.HttpApi.Hosting
{
    public class ZenAuditActionFilter : IAsyncActionFilter, ITransientDependency
    {
        private readonly IAuditingHelper _auditingHelper;
        private readonly IAuditingManager _auditingManager;
        private readonly ICurrentUser _currentUser;

        public ZenAuditActionFilter(IOptions<AbpAuditingOptions> options,
            IAuditingHelper auditingHelper,
            IAuditingManager auditingManager,
            ICurrentUser currentUser)
        {
            Options = options.Value;
            _auditingHelper = auditingHelper;
            _auditingManager = auditingManager;
            _currentUser = currentUser;
        }

        protected AbpAuditingOptions Options { get; }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!ShouldSaveAudit(context, out var auditLog, out var auditLogAction))
            {
                await next();
                return;
            }

            using (AbpCrossCuttingConcerns.Applying(context.Controller, AbpCrossCuttingConcerns.Auditing))
            {
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    var result = await next();

                    if (result.Exception != null && !result.ExceptionHandled)
                    {
                        auditLog.Exceptions.Add(result.Exception);
                    }
                }
                catch (Exception ex)
                {
                    auditLog.Exceptions.Add(ex);
                    throw;
                }
                finally
                {
                    stopwatch.Stop();
                    auditLogAction.ExecutionDuration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
                    auditLog.Actions.Add(auditLogAction);
                }
            }
        }

        private bool ShouldSaveAudit(ActionExecutingContext context, out AuditLogInfo auditLog,
            out AuditLogActionInfo auditLogAction)
        {
            auditLog = null;
            auditLogAction = null;
            var method = context.HttpContext.Request.Method;
            if (method.Equals(HttpMethod.Options.Method, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (!Options.IsEnabled)
            {
                return false;
            }

            if (!context.ActionDescriptor.IsControllerAction())
            {
                return false;
            }

            var auditLogScope = _auditingManager.Current;
            if (auditLogScope == null)
            {
                return false;
            }

            if (!_auditingHelper.ShouldSaveAudit(context.ActionDescriptor.GetMethodInfo(), true))
            {
                return false;
            }

            auditLog = auditLogScope.Log;
            auditLog.UserId = _currentUser?.Id;
            auditLog.UserName = _currentUser?.UserName;
            auditLogAction = _auditingHelper.CreateAuditLogAction(
                auditLog,
                context.ActionDescriptor.AsControllerActionDescriptor().ControllerTypeInfo.AsType(),
                context.ActionDescriptor.AsControllerActionDescriptor().MethodInfo,
                context.ActionArguments
            );

            return true;
        }
    }
}