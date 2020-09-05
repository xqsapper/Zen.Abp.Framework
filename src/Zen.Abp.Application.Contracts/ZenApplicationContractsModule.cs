using Volo.Abp.Modularity;
using Zen.Abp.Domain.Shared;

// ReSharper disable once CheckNamespace
namespace Zen.Abp.Application.Contracts
{
    [DependsOn(typeof(ZenDomainSharedModule))]
    public class ZenApplicationContractsModule : AbpModule
    {
    }
}