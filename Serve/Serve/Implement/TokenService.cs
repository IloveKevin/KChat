using Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implement
{
    public class TokenService : ITokenService
    {
        private readonly IOptions<JWTConfigration> _options;
        public TokenService(IOptions<JWTConfigration> options)
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
                AudienceValidator = (audiences, securityToken, validationParameters) =>
                {
                    Console.WriteLine(audiences.FirstOrDefault());
                    return true;
                },
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Secret)),
            };
			options.Events = new JwtBearerEvents
			{
				OnMessageReceived = context =>
				{
					var accessToken = context.Request.Query["access_token"];
                    Console.WriteLine(accessToken);
					// If the request is for our hub...
					var path = context.HttpContext.Request.Path;
					Console.WriteLine(path.ToString());
					if (!string.IsNullOrEmpty(accessToken) &&
						(path.StartsWithSegments("/chathub")))
					{
						// Read the token out of the query string
						context.Token = accessToken;
					}
					return Task.CompletedTask;
				}
			};
		}

        /// <summary>
        /// 签发token
        /// </summary>
        public string IssuaToken(long expires, Claim[] claims)
        {
            var _expires = Convert.ToDouble(expires);
            var token = new JwtSecurityToken(
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
