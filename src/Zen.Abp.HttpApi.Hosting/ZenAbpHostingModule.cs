using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Zen.Abp.Application.Contracts;

// ReSharper disable once CheckNamespace
namespace Zen.Abp.HttpApi.Hosting
{
    public class ZenAbpHostingModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddTransient<IZenAuthenticationService, ZenNullAuthenticationService>();
        }
    }
}