// ReSharper disable once CheckNamespace

namespace Zen.Abp.Domain.Shared
{
    public static class ZenSharedErrorCodes
    {
        /// <summary>
        ///     响应失败
        /// </summary>
        public static readonly string Failed = ((int) ApiResultCode.Failed).ToString();

        /// <summary>
        ///     响应成功
        /// </summary>
        public static readonly string Success = ((int) ApiResultCode.Success).ToString();

        /// <summary>
        ///     账号被锁定
        /// </summary>
        public static readonly string AccountLocked = ((int) ApiResultCode.AccountLocked).ToString();

        /// <summary>
        ///     账号被禁用
        /// </summary>
        public static readonly string AccountDisabled = ((int) ApiResultCode.AccountDisabled).ToString();

        /// <summary>
        ///     Token过期
        /// </summary>
        public static readonly string TokenExpired = ((int) ApiResultCode.TokenExpired).ToString();

        /// <summary>
        ///     请求token错误
        /// </summary>
        public static readonly string TokenInvalid = ((int) ApiResultCode.TokenInvalid).ToString();

        /// <summary>
        ///     Token无访问权限
        /// </summary>
        public static readonly string TokenNotAllow = ((int) ApiResultCode.TokenNotAllow).ToString();

        /// <summary>
        ///     请求参数不正确
        /// </summary>
        public static readonly string ParametersInvalid = ((int) ApiResultCode.ParametersInvalid).ToString();
    }

    public enum ApiResultCode
    {
        /// <summary>
        ///     响应失败
        /// </summary>
        Failed = 0,

        /// <summary>
        ///     响应成功
        /// </summary>
        Success = 1,

        /// <summary>
        ///     账号被锁定
        /// </summary>
        AccountLocked = 101,

        /// <summary>
        ///     账号被禁用
        /// </summary>
        AccountDisabled = 102,

        /// <summary>
        ///     Token过期
        /// </summary>
        TokenExpired = 104,

        /// <summary>
        ///     请求token错误
        /// </summary>
        TokenInvalid = 105,

        /// <summary>
        ///     请求token无权限
        /// </summary>
        TokenNotAllow = 106,

        /// <summary>
        ///     请求参数不正确
        /// </summary>
        ParametersInvalid = 201
    }
}