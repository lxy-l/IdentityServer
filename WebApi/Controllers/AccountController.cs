using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace WebApi.Controllers
{
    /// <summary>
    /// 用户
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        /// <summary>
        /// 获取token
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AcessToken()
        {
            await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            return null;
        }
    }
}
