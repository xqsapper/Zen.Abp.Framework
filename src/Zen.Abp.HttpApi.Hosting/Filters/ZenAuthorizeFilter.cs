using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using Volo.Abp;
using Volo.Abp.Security.Claims;
using Zen.Abp.Application.Contracts;
using Zen.Abp.Domain.Shared;

// ReSharper disable once CheckNamespace
namespace Zen.Abp.HttpApi.Hosting
{
    public class ZenAuthorizeFilter : ActionFilterAttribute
    {
        private const string TokenType = "Bearer ";
        private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IZenAuthenticationService _zenService;

        public ZenAuthorizeFilter(ICurrentPrincipalAccessor currentPrincipalAccessor,
            IHttpContextAccessor httpContextAccessor,
            IZenAuthenticationService zenService)
        {
            _currentPrincipalAccessor = currentPrincipalAccessor;
            _httpContextAccessor = httpContextAccessor;
            _zenService = zenService;
            Order = int.MinValue;
        }

        /// <summary>
        ///     在方法执行前
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var authorizeAttrs = GetAuthorizeAttrs(filterContext);
            if (!authorizeAttrs.Any())
            {
                return;
            }

            //token获取
            var accessToken = GetAccessToken(filterContext);
            _httpContextAccessor.HttpContext.Items[ZenSharedConstants.AccessToken] = accessToken;
            //应用编码
            var appCode = _zenService.GetAppCode().GetAwaiter().GetResult();
            //身份设置
            var claimsIdentity = _zenService.GetClaimsIdentity(accessToken, appCode).GetAwaiter().GetResult();
            _currentPrincipalAccessor.Principal.AddIdentity(claimsIdentity);
            //权限校验
            var method = filterContext.HttpContext.Request.Method;
            var route = filterContext.ActionDescriptor.GetMethodInfo().GetCustomAttribute<RouteAttribute>();
            var templateUrl = route?.Template ?? string.Empty;
            _zenService.CheckPermission(method, templateUrl, accessToken, appCode).GetAwaiter().GetResult();
        }

        #region 私有方法

        private ICollection<ZenAuthorizeAttribute> GetAuthorizeAttrs(ActionExecutingContext filterContext)
        {
            var result = new List<ZenAuthorizeAttribute>(0);
            if (filterContext.Controller.GetType().GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any())
            {
                return result;
            }

            //判断Action是否需要验证
            if (filterContext.ActionDescriptor is ControllerActionDescriptor ctrlActionDescriptor)
            {
                var authorizeAttrs = ctrlActionDescriptor.MethodInfo
                    .GetCustomAttributes(typeof(ZenAuthorizeAttribute), true)
                    .Cast<ZenAuthorizeAttribute>().ToArray();
                if (authorizeAttrs.Any())
                {
                    result.AddRange(authorizeAttrs);
                }
            }

            return result;
        }

        private string GetAccessToken(ActionExecutingContext filterContext)
        {
            var accessToken = filterContext.HttpContext.Request.Headers[HeaderNames.Authorization].ToString();
            if (accessToken.StartsWith(TokenType, StringComparison.OrdinalIgnoreCase))
            {
                accessToken = accessToken.Substring(TokenType.Length).Trim();
            }

            if (string.IsNullOrEmpty(accessToken))
            {
                throw new BusinessException(ZenSharedErrorCodes.TokenInvalid, "accessToken缺失");
            }

            return accessToken.ToLowerInvariant();
        }

        #endregion 私有方法
    }
}