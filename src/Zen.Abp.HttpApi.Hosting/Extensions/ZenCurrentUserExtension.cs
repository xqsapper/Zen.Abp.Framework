using System;
using System.Collections.Generic;
using System.Security.Claims;
using Volo.Abp.Security.Claims;
using Zen.Abp.Domain.Shared;

// ReSharper disable once CheckNamespace
namespace Zen.Abp.HttpApi.Hosting
{
    public static class ZenCurrentUserExtension
    {
        public static ClaimsIdentity BuildClaimsIdentity(CurrentUserModel currentUser)
        {
            var abpClaims = BuildClaims(currentUser);
            var result = new ClaimsIdentity(abpClaims);
            return result;
        }

        public static List<Claim> BuildClaims(CurrentUserModel currentUser)
        {
            currentUser.TrimStringProps();
            currentUser.Roles ??= new string[0];
            var abpClaims = new List<Claim>
            {
                new Claim(AbpClaimTypes.UserId, currentUser.AccountId.ToString()),
                new Claim(AbpClaimTypes.EditionId, currentUser.AccountId.ToString()),
                new Claim(AbpClaimTypes.UserName, currentUser.LoginName),
                new Claim(ZenSharedConstants.OwnerIdClaim, currentUser.OwnerId.HasValue
                    ? currentUser.OwnerId.ToString()
                    : ""),
                new Claim(ZenSharedConstants.OrgIdClaim, currentUser.OrgId.HasValue
                    ? currentUser.OrgId.ToString()
                    : ""),
                new Claim(ZenSharedConstants.OrgNameClaim, currentUser.OrgName),
                new Claim(ZenSharedConstants.NickNameClaim, currentUser.NickName),
                new Claim(AbpClaimTypes.Role, string.Join(',', currentUser.Roles)),
                new Claim(AbpClaimTypes.Email, currentUser.Email),
                new Claim(AbpClaimTypes.PhoneNumber, currentUser.Phone),
                new Claim(ZenSharedConstants.AccessToken, currentUser.AccessToken)
            };
            return abpClaims;
        }
    }


    public class CurrentUserModel
    {
        /// <summary>
        ///     登录账号的Id
        /// </summary>
        public Guid AccountId { get; set; }

        /// <summary>
        ///     拥有者Id
        /// </summary>
        public Guid? OwnerId { get; set; }


        /// <summary>
        ///     组织Id
        /// </summary>
        public Guid? OrgId { get; set; }

        /// <summary>
        ///     组织名称
        /// </summary>
        public string OrgName { get; set; }


        /// <summary>
        ///     登录名
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        ///     昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        ///     邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     手机号
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        ///     角色列表
        /// </summary>
        public string[] Roles { get; set; }


        /// <summary>
        ///     当前请求的Token
        /// </summary>
        public string AccessToken { get; set; }
    }
}