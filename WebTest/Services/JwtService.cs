using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WebTest.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 生成JwtToken
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public JwtTokenViewModel GetJwtToken(List<Claim> claims=null)
        {
            if (claims==null)
            {
                throw new Exception("用户信息为空！");
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSetting:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var time = TimeSpan.FromSeconds(3600);
            var token = new JwtSecurityToken
            (
                issuer: _configuration["JwtSetting:Issuer"], 
                audience: _configuration["JwtSetting:Audience"], 
                claims: claims, expires: DateTime.Now.Add(time), 
                signingCredentials: creds
            );
            var access_token = new JwtSecurityTokenHandler().WriteToken(token);
            return new JwtTokenViewModel {access_token= access_token, expires_in=time.TotalSeconds,token_type=JwtBearerDefaults.AuthenticationScheme};

        }

        /// <summary>
        /// 获取用户id
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public string GetUserId(ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(x=>x.Type==ClaimTypes.SerialNumber)?.Value;
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="jwtStr"></param>
        /// <returns></returns>
        public JwtSecurityToken SerializeJwt(string jwtStr)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(jwtStr);   
            return jwtToken;
        }

        /// <summary>
        /// JwtToken视图模型
        /// </summary>
        public class JwtTokenViewModel
        {
            public string access_token { get; set; }
            public double expires_in { get; set; }
            public string token_type { get; set; }
        }
    }
}
