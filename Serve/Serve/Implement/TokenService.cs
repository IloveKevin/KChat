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
using Util.Enum;
using Util.Redis;

namespace Service.Implement
{
	public class TokenService : ITokenService
	{
		private readonly IOptions<JWTConfigration> _options;
		private readonly IRedisHelper _redisHelper;
		public TokenService(IOptions<JWTConfigration> options, IRedisHelper redisHelper)
		{
			_options = options;
			_redisHelper = redisHelper;
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
				},
				OnTokenValidated = async context =>
				{
					var jwtToken = context.SecurityToken as JwtSecurityToken;
					var userId = jwtToken?.Claims.FirstOrDefault(c => c.Type == "ID");
					if (userId == null)
					{
						context.Fail("Invalid token");
						return;
					}
					var token = await _redisHelper.GetAsync($"{ERedisKey.AccessToken}_{userId.Value}");
					if (token == null || token != jwtToken?.RawData)
					{
						context.Fail("Invalid token");
						return;
					}
					return;
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
		public JwtSecurityToken? Get(string token)
		{
			try
			{
				return new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
			}
			catch (Exception ex)
			{
				return null;
			}
		}
	}
}
