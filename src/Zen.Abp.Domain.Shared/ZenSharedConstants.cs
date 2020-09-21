using System;

// ReSharper disable once CheckNamespace
namespace Zen.Abp.Domain.Shared
{
    public static class ZenSharedConstants
    {
        public const string AccessToken = "access_token";
        public const string NickNameClaim = "nick_name";
        public const string OwnerIdClaim = "owner_id";
        public const string OrgIdClaim = "org_id";
        public const string OrgNameClaim = "org_name";

        /// <summary>
        ///     默认缺省时间
        /// </summary>
        public static readonly DateTime DefaultTime = new DateTime(1970, 1, 1);
    }
}