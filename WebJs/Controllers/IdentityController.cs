using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Linq;

namespace WebJs.Controllers
{
    /// <summary>
    /// 权限
    /// </summary>
    [Route("[controller]")]
    [Authorize]
    public class IdentityController : ControllerBase
    {
        /// <summary>
        /// 获取权限
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public IActionResult Get()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }
    }
}
