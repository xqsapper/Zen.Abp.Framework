using System;
using Volo.Abp.Users;
using Zen.Abp.Domain.Shared;

// ReSharper disable once CheckNamespace
namespace Zen.Abp.Application.Contracts
{
    public interface IZenCurrentUser
    {
        /// <summary>
        /// 登录账号的Id
        /// </summary>
        public Guid AccountId { get; }

        /// <summary>
        /// 拥有者Id
        /// </summary>
        public Guid? OwnerId { get; }

        /// <summary>
        /// 租户Id
        /// </summary>
        public Guid? TenantId { get; }

        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginName { get; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; }

        /// <summary>
        /// 角色列表
        /// </summary>
        public string[] Roles { get; }


        /// <summary>
        /// 当前请求的Token
        /// </summary>
        public string AccessToken { get; }

        /// <summary>
        /// 是否包含某角色
        /// </summary>
        /// <param name="roleCode">角色编码</param>
        /// <returns></returns>
        public bool IsInRole(string roleCode);
    }


    public class ZenCurrentUser : IZenCurrentUser
    {
        private readonly ICurrentUser _currentUser;

        public ZenCurrentUser(ICurrentUser currentUser)
        {
            _currentUser = currentUser;
        }

        /// <summary>
        /// 登录账号的Id
        /// </summary>
        public Guid AccountId => _currentUser.Id.GetValueOrDefault();

        /// <summary>
        /// 拥有者Id
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
        /// 租户Id
        /// </summary>
        public Guid? TenantId => _currentUser.TenantId;

        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginName => _currentUser.UserName;

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName => _currentUser.FindClaim(ZenSharedConstants.NickNameClaim).Value;

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email => _currentUser.Email;

        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone => _currentUser.PhoneNumber;

        /// <summary>
        /// 角色列表
        /// </summary>
        public string[] Roles => _currentUser.Roles;

        /// <summary>
        /// 当前请求的Token
        /// </summary>
        public string AccessToken => _currentUser.FindClaim(ZenSharedConstants.AccessToken).Value;

        /// <summary>
        /// 是否包含某角色
        /// </summary>
        /// <param name="roleCode">角色编码</param>
        /// <returns></returns>
        public bool IsInRole(string roleCode)
        {
            return _currentUser.IsInRole(roleCode);
        }
    }
}