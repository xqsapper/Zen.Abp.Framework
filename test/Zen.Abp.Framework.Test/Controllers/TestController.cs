using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Volo.Abp.AspNetCore.Mvc;
using Zen.Abp.Domain.Shared;

namespace Zen.Abp.Framework.Test.Controllers
{
    [ApiController]
    [OpenApiTag("test", Description = "测试相关接口")]
    public class TestController : AbpController
    {
        /// <summary>
        /// 测试接口
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("bbs/v1/test")]
        [OpenApiOperation("test")]
        public Task<string> Test([FromQuery] [Required] Guid number)
        {
            return Task.FromResult(number.ToString());
        }
    }
}