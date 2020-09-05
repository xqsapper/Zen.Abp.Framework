using System;

// ReSharper disable once CheckNamespace
namespace Zen.Abp.HttpApi.Hosting
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ZenAuthorizeAttribute : Attribute
    {
        public ZenAuthorizeAttribute()
        {
        }

        public ZenAuthorizeAttribute(string policy)
        {
            Policy = policy;
        }

        /// <summary>
        ///     策略名称，单个
        /// </summary>
        public string Policy { get; set; } = string.Empty;

        /// <summary>
        ///     角色名称，多个以英文逗号分隔
        /// </summary>
        public string Roles { get; set; } = string.Empty;

        /// <summary>
        ///     账号登录名，多个以英文逗号分隔
        /// </summary>
        public string Names { get; set; } = string.Empty;

        /// <summary>
        ///     账号类型，多个以英文逗号分隔
        /// </summary>
        public string Types { get; set; } = string.Empty;
    }
}