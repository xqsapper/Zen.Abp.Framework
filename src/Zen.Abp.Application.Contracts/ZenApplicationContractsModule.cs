using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Zen.Abp.Domain.Shared;

// ReSharper disable once CheckNamespace
namespace Zen.Abp.Application.Contracts
{
    [DependsOn(typeof(ZenDomainSharedModule))]
    public class ZenApplicationContractsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddTransient<IZenCurrentUser, ZenCurrentUser>();
        }
    }
}