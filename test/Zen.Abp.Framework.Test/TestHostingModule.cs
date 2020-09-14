using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Auditing;
using Volo.Abp.AspNetCore.Mvc.ExceptionHandling;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Zen.Abp.HttpApi.Hosting;

namespace Zen.Abp.Framework.Test
{
    [DependsOn(typeof(AbpAutofacModule),
        typeof(AbpAspNetCoreSerilogModule),
        typeof(AbpAspNetCoreMvcModule),
        typeof(ZenAbpHostingModule)
    )]
    public class TestHostingModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            context.Services.AddZenNSwag("TestService", "测试服务");
          
            //context.Services.AddAuthorization();
            Configure<MvcOptions>(options =>
            {
                var removeAbpFilters = new[] {typeof(AbpAuditActionFilter), typeof(AbpExceptionFilter)};
                options.Filters.RemoveAll(m => removeAbpFilters.Contains(m.GetType()));
                // 替换ExceptionFilter
                options.Filters.Add(typeof(ZenExceptionFilter));
                //授权过滤器添加
                options.Filters.Add(typeof(ZenAuthorizeFilter));
                // 审计日志过滤器
                options.Filters.Add(typeof(ZenAuditActionFilter));

            });
            context.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder => builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .SetIsOriginAllowed(host => true));
            });
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCorrelationId();
            app.UseVirtualFiles();
            app.UseRouting();
            //app.UseAuthentication();
            //app.UseAuthorization();
            app.UseCors();
            app.UseAbpSerilogEnrichers();
            app.UseConfiguredEndpoints();
            app.UseZenNSwagUi();
            app.UseZenNSwagIndex();
            app.UseZenWinService(new ZenWinServiceOption
            {
                Executable = "Zen.Abp.Framework.Test.exe",
                Name = "Zen.Abp.Framework.Test"
            });
        }
    }
}