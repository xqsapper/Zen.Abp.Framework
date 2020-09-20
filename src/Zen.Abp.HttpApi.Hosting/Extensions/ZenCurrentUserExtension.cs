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
        public static ClaimsIdentity BuildClaimsIdentity(Guid accountId, string loginName,
            Guid? ownerId, string nickName, ICollection<string> roleCodes,
            string email, string phone, string accessToken)
        {
            var abpClaims = BuildClaim(accountId, loginName, ownerId, nickName,
                roleCodes, email, phone, accessToken);
            var result = new ClaimsIdentity(abpClaims);
            return result;
        }

        public static List<Claim> BuildClaim(Guid accountId, string loginName,
            Guid? ownerId, string nickName, ICollection<string> roleCodes,
            string email, string phone, string accessToken)
        {

          
            roleCodes ??= new string[0];
            var abpClaims = new List<Claim>
            {
                new Claim(AbpClaimTypes.UserId, accountId.ToString()),
                new Claim(AbpClaimTypes.EditionId, accountId.ToString()),
                new Claim(AbpClaimTypes.UserName, loginName.EnsureTrim()),
                new Claim(ZenSharedConstants.OwnerIdClaim, ownerId.HasValue ? ownerId.ToString() : ""),
                new Claim(ZenSharedConstants.NickNameClaim, nickName.EnsureTrim()),
                new Claim(AbpClaimTypes.Role, string.Join(',', roleCodes)),
                new Claim(AbpClaimTypes.Email, email.EnsureTrim()),
                new Claim(AbpClaimTypes.PhoneNumber, phone.EnsureTrim()),
                new Claim(ZenSharedConstants.AccessToken, accessToken.EnsureTrim())
            };
            return abpClaims;
        }
    }
}