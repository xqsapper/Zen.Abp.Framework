using System;

// ReSharper disable once CheckNamespace
namespace Zen.Abp.Domain.Shared
{
    public static class ZenSharedConstants
    {
        public const string AccessToken = "zen_access_token";

        /// <summary>
        ///     默认缺省时间
        /// </summary>
        public static readonly DateTime DefaultTime = new DateTime(1970, 1, 1);
    }
}