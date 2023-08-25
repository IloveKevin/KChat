using Config;
using JWT.Algorithms;
using JWT.Serializers;
using JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Util.JWT
{
    public class JWTHelper : IJWTHelper
    {
        private readonly IOptions<JWTConfigration> _options;
        public JWTHelper(IOptions<JWTConfigration> options)
        {
            _options = options;
        }

        /// <summary>
        /// 配置JWT
        /// </summary>
        /// <param name="options"></param>
        public void OnConfigration(JwtBearerOptions options)
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            var config = _options.Value;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config.Issuer, // 更换为实际的发行者
                ValidAudience = config.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Secret)),
            };
		}

        /// <summary>
        /// 签发token
        /// </summary>
        public string IssuaToken(long expires, Claim[] claims)
        {
            var _expires= Convert.ToDouble(expires);
			var token= new JwtSecurityToken(
				_options.Value.Issuer,
				_options.Value.Audience,
				claims,
				DateTime.Now,
				DateTime.Now.AddMinutes(_expires),
				new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.Secret)), SecurityAlgorithms.HmacSha256)
				);
			return new JwtSecurityTokenHandler().WriteToken(token);
		}

        /// <summary>
        /// 获取token数据
        /// </summary>
        public SecurityToken? Get(string token)
        {
            try
            {
				return new JwtSecurityTokenHandler().ReadToken(token);
			}
			catch (Exception ex)
            {
                return null;
            }
        }
    }
}
