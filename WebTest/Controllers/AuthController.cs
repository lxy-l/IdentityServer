using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using IdentityModel.Client;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;

using WebTest.Data;
using WebTest.Services;

namespace WebTest.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly UserContext _db;
        public AuthController(JwtService jwtService,UserContext context)
        {
            _jwtService = jwtService;
            _db = context;
        }

        [HttpPost]
        [Route("JwtToken")]
        public IActionResult JwtToken(string name,string pass)
        {
            var user = _db.Users.FirstOrDefault(x => x.Name == name && x.PasswordHash == pass);
            if (user!=null)
            {
                var claims = new Claim[] { 
                    new Claim(ClaimTypes.Name, user.Name), 
                    new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()), 
                    new Claim(ClaimTypes.SerialNumber, user.Id.ToString()) 
                };
                return Ok(_jwtService.GetJwtToken(claims));
            }
            else
            {
                return Unauthorized(new {error="用户验证失败" });
            }
        }
        [HttpPost]
        [Route("RefreshToken")]
        [Authorize]
        public IActionResult RefreshToken()
        {
            var tokenModel = _jwtService.SerializeJwt(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            if (tokenModel!=null)
            {
                var user = _db.Users.Find(int.Parse(tokenModel.Id));
                if (user!=null)
                {
                    var claims = new Claim[] { 
                        new Claim(ClaimTypes.Name, user.Name), 
                        new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()), 
                        new Claim(ClaimTypes.SerialNumber, user.Id.ToString()) 
                    };
                    return Ok(_jwtService.GetJwtToken(claims));
                }
                else
                {
                    return Unauthorized(new { error="用户验证失败"});
                }
            }
            return Unauthorized(new { error="token解析失败"});
        }

    }
}
