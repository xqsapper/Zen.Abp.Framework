using System.Security.Claims;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Zen.Abp.Application.Contracts
{
    public interface IZenAuthenticationService
    {
        /// <summary>
        ///     获取应用编码
        /// </summary>
        /// <returns></returns>
        Task<string> GetAppCode();

        /// <summary>
        ///     获取身份信息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="appCode">应用编码</param>
        /// <returns></returns>
        Task<ClaimsIdentity> GetClaimsIdentity(string accessToken, string appCode);

        /// <summary>
        ///     检查权限
        /// </summary>
        /// <param name="requestMethod">请求的方法</param>
        /// <param name="requestRoute">路由模板</param>
        /// <param name="accessToken">Token</param>
        /// <param name="appCode">应用编码</param>
        /// <returns></returns>
        Task CheckPermission(string requestMethod, string requestRoute, string accessToken, string appCode);
    }

    /// <summary>
    /// 空的授权服务
    /// </summary>
    public class ZenNullAuthenticationService : IZenAuthenticationService
    {
        public Task CheckPermission(string requestMethod, string requestRoute, string accessToken, string appCode)
        {
            return Task.CompletedTask;
        }

        public Task<string> GetAppCode()
        {
            return Task.FromResult(string.Empty);
        }

        public Task<ClaimsIdentity> GetClaimsIdentity(string accessToken, string appCode)
        {
            return Task.FromResult(new ClaimsIdentity());
        }
    }
}