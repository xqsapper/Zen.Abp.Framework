using System;
using Volo.Abp.Caching;

// ReSharper disable once CheckNamespace
namespace Zen.Abp.Domain.Shared
{
    [CacheName("zen.ApiResultT")]
    public class ApiResult<T> : ApiResultBase
    {
        /// <summary>
        ///     返回数据
        /// </summary>
        public T Data { get; set; }
    }

    public class ApiResult : ApiResultBase
    {
        /// <summary>
        ///     返回数据
        /// </summary>
        public object Data { get; set; }
    }

    public class ApiResultBase
    {
        /// <summary>
        ///     返回码
        /// </summary>
        public int Code { get; set; } = (int) ApiResultCode.Success;

        /// <summary>
        ///     错误消息
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        ///     服务端时间
        /// </summary>
        public DateTime ServerTime => DateTime.Now;
    }
}