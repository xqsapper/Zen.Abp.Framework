using System;
using System.Security.Claims;
using Volo.Abp.Users;
using Zen.Abp.Domain.Shared;

// ReSharper disable once CheckNamespace
namespace Zen.Abp.Application.Contracts
{
    public interface IZenCurrentUser
    {
        /// <summary>
        ///     登录账号的Id
        /// </summary>
        public Guid AccountId { get; }

        /// <summary>
        ///     拥有者Id
        /// </summary>
        public Guid? OwnerId { get; }


        /// <summary>
        ///     组织Id
        /// </summary>
        public Guid? OrgId { get; }

        /// <summary>
        ///     组织名称
        /// </summary>
        public string OrgName { get; }


        /// <summary>
        ///     租户Id
        /// </summary>
        public Guid? TenantId { get; }

        /// <summary>
        ///     登录名
        /// </summary>
        public string LoginName { get; }

        /// <summary>
        ///     昵称
        /// </summary>
        public string NickName { get; }

        /// <summary>
        ///     邮箱
        /// </summary>
        public string Email { get; }

        /// <summary>
        ///     手机号
        /// </summary>
        public string Phone { get; }

        /// <summary>
        ///     角色列表
        /// </summary>
        public string[] Roles { get; }


        /// <summary>
        ///     当前请求的Token
        /// </summary>
        public string AccessToken { get; }


        public bool IsInRole(string roleName);

        Claim FindClaim(string claimType);

        Claim[] FindClaims(string claimType);

        Claim[] GetAllClaims();
    }


    public class ZenCurrentUser : IZenCurrentUser
    {
        private readonly ICurrentUser _currentUser;

        public ZenCurrentUser(ICurrentUser currentUser)
        {
            _currentUser = currentUser;
        }

        /// <summary>
        ///     登录账号的Id
        /// </summary>
        public Guid AccountId => _currentUser.Id.GetValueOrDefault();

        /// <summary>
        ///     拥有者Id
        /// </summary>
        public Guid? OwnerId
        {
            get
            {
                var value = _currentUser.FindClaim(ZenSharedConstants.OwnerIdClaim).Value;
                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                return Guid.Parse(value);
            }
        }

        /// <summary>
        ///     租户Id
        /// </summary>
        public Guid? TenantId => _currentUser.TenantId;

        /// <summary>
        ///     登录名
        /// </summary>
        public string LoginName => _currentUser.UserName;

        /// <summary>
        ///     昵称
        /// </summary>
        public string NickName => _currentUser.FindClaim(ZenSharedConstants.NickNameClaim).Value;

        /// <summary>
        ///     邮箱
        /// </summary>
        public string Email => _currentUser.Email;

        /// <summary>
        ///     手机号
        /// </summary>
        public string Phone => _currentUser.PhoneNumber;

        /// <summary>
        ///     角色列表
        /// </summary>
        public string[] Roles => _currentUser.Roles;

        /// <summary>
        ///     当前请求的Token
        /// </summary>
        public string AccessToken => _currentUser.FindClaim(ZenSharedConstants.AccessToken).Value;

        /// <summary>
        ///     机构Id
        /// </summary>
        public Guid? OrgId
        {
            get
            {
                var value = _currentUser.FindClaim(ZenSharedConstants.OrgIdClaim).Value;
                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                return Guid.Parse(value);
            }
        }

        /// <summary>
        ///     机构名称
        /// </summary>
        public string OrgName => _currentUser.FindClaim(ZenSharedConstants.OrgNameClaim).Value;

        public bool IsInRole(string roleName)
        {
            return _currentUser.IsInRole(roleName);
        }

        public Claim FindClaim(string claimType)
        {
            return _currentUser.FindClaim(claimType);
        }

        public Claim[] FindClaims(string claimType)
        {
            return _currentUser.FindClaims(claimType);
        }

        public Claim[] GetAllClaims()
        {
            return _currentUser.GetAllClaims();
        }
    }
}