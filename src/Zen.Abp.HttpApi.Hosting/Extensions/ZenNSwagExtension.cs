using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Namotion.Reflection;
using NSwag;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

// ReSharper disable once CheckNamespace
namespace Zen.Abp.HttpApi.Hosting
{
    public static class ZenNSwagExtension
    {
        public static IServiceCollection AddZenNSwag(this IServiceCollection services, string title,
            string description)
        {
            var tokenType = "Bearer";
            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = title;
                    document.Info.Description = description;
                    document.Info.Contact = new OpenApiContact
                    {
                        Name = "zen-arch",
                        Email = "developer@zen-arch.com.cn"
                    };

                    document.SecurityDefinitions.Add(tokenType, new OpenApiSecurityScheme
                    {
                        Description = "请输入JWT格式的鉴权Token，示例格式为: Bearer+英文空格+AccessToke",
                        Name = "Authorization",
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        In = OpenApiSecurityApiKeyLocation.Header
                    });
                };
                config.OperationProcessors.Insert(0, new ZenOperationProcessor());
                //config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor(tokenType));
                //config.OperationProcessors.Add(new CsgOperationSecurityScopeProcessor(tokenType));
                config.OperationProcessors.Add(new ZenAspNetCoreOperationSecurityScopeProcessor(tokenType));
            });

            return services;
        }

        public static IApplicationBuilder UseZenNSwagUi(this IApplicationBuilder app)
        {
            app.UseOpenApi(); // serve OpenAPI/Swagger documents
            app.UseSwaggerUi3(); // serve Swagger UI
            app.UseReDoc(); // serve ReDoc UI
            return app;
        }

        public static IApplicationBuilder UseZenNSwagIndex(this IApplicationBuilder app)
        {
            //默认起始页设置
            app.Run(ctx =>
            {
                var pathUrl = ctx.Request.Path.Value.TrimEnd('/');
                if (string.IsNullOrWhiteSpace(pathUrl))
                {
                    ctx.Response.Redirect("/swagger/index.html");
                }

                return Task.FromResult(0);
            });
            return app;
        }
    }

    public class ZenOperationProcessor : IOperationProcessor
    {
        public bool Process(OperationProcessorContext context)
        {
            var fullName = context.ControllerType.FullName ?? string.Empty;
            var abpCtrl = fullName.Contains("Volo.Abp.", StringComparison.OrdinalIgnoreCase);
            return !abpCtrl;
        }
    }

    public class ZenAspNetCoreOperationSecurityScopeProcessor : IOperationProcessor
    {
        private readonly string _name;

        public ZenAspNetCoreOperationSecurityScopeProcessor() : this("Bearer")
        {
        }

        public ZenAspNetCoreOperationSecurityScopeProcessor(string name)
        {
            _name = name;
        }

        public bool Process(OperationProcessorContext context)
        {
            var aspNetCoreContext = (AspNetCoreOperationProcessorContext) context;
            var endpointMetadata = aspNetCoreContext?.ApiDescription?.ActionDescriptor
                ?.TryGetPropertyValue<IList<object>>("EndpointMetadata");
            if (endpointMetadata != null)
            {
                var allowAnonymous = endpointMetadata.OfType<AllowAnonymousAttribute>().Any();
                if (allowAnonymous)
                {
                    return true;
                }

                var authorizeAttributes = endpointMetadata.OfType<ZenAuthorizeAttribute>().ToList();
                if (!authorizeAttributes.Any())
                {
                    return true;
                }

                if (context.OperationDescription.Operation.Security == null)
                {
                    context.OperationDescription.Operation.Security = new List<OpenApiSecurityRequirement>();
                }

                var scopes = GetScopes(authorizeAttributes);
                if (scopes != null && scopes.Any())
                {
                    context.OperationDescription.Operation.Security.Add(new OpenApiSecurityRequirement
                    {
                        {_name, scopes}
                    });
                }
            }

            return true;
        }

        protected virtual IEnumerable<string> GetScopes(IEnumerable<ZenAuthorizeAttribute> authorizeAttributes)
        {
            return authorizeAttributes
                .Where(a => a.Roles != null)
                .SelectMany(a => a.Roles.Split(','))
                .Distinct();
        }
    }
}