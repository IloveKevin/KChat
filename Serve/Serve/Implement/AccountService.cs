using Config;
using EFCore.Entity;
using KChatServe.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Model.HttpModel.Response;
using Model.JWT;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Util.Enum;
using Util.Redis;

namespace Service.Implement
{
	public class AccountService : IAccountService
	{
		private readonly MySqlServerDataBaseContext _mySqlServerDataBaseContext;
		private readonly ITokenService _tokenService;
		private readonly IOptions<JWTConfigration> _jwtOption;
		private readonly IRedisHelper _redisHelper;

		public AccountService(MySqlServerDataBaseContext mySqlServerDataBaseContext
			,ITokenService tokenService
			,IOptions<JWTConfigration> jwtOption
			,IRedisHelper redisHelper)
		{
			_mySqlServerDataBaseContext = mySqlServerDataBaseContext;
			_tokenService = tokenService;
			_jwtOption = jwtOption;
			_redisHelper = redisHelper;
		}
		Token? IAccountService.Login(string account, string password)
		{
			var user = _mySqlServerDataBaseContext._users.FirstOrDefault(u => u.Account == account && u.Password == password);
			if (user == null)
			{
				return null;
			}
			var claims = new[]
			{
				new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
			};
			var accessToken = _tokenService.IssuaToken(_jwtOption.Value.AccessExpiration,claims);
			var refreshToken = _tokenService.IssuaToken(_jwtOption.Value.RefreshExpiration, claims);
			 _redisHelper.Set($"{ERedisKey.AccessToken}_{user.Id}",accessToken);
			 _redisHelper.Set($"{ERedisKey.RefreshToken}_{user.Id}", refreshToken);
			return new Token()
			{
				AccessToken = accessToken,
				RefreshToken = refreshToken
			};
		}

		async Task<Token?> IAccountService.LoginAsync(string account, string password)
		{
			var user = await _mySqlServerDataBaseContext._users.FirstOrDefaultAsync(u => u.Account == account && u.Password == password);
			if (user == null)
			{
				return null;
			}
			var claims = new[]
			{
				new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
			};
			var accessToken = _tokenService.IssuaToken(_jwtOption.Value.AccessExpiration, claims);
			var refreshToken = _tokenService.IssuaToken(_jwtOption.Value.RefreshExpiration, claims);
			await _redisHelper.SetAsync($"{ERedisKey.AccessToken}_{user.Id}", accessToken);
			await _redisHelper.SetAsync($"{ERedisKey.RefreshToken}_{user.Id}", refreshToken);
			return new Token()
			{
				AccessToken = accessToken,
				RefreshToken = refreshToken
			};
		}
		

		public User? Register(string account, string password)
		{
			var user = _mySqlServerDataBaseContext._users.FirstOrDefault(u => u.Account == account);
			if (user == null)
			{
				_mySqlServerDataBaseContext._users.Add(new User()
				{
					Account = account,
					Password = password
				});
				var userId = _mySqlServerDataBaseContext.SaveChanges();
				return new User()
				{
					Id = userId,
					Account = account,
					Password = password
				};
			}
			return null;
		}

		public async Task<User?> RegisterAsync(string account, string password)
		{
			var user = await _mySqlServerDataBaseContext._users.FirstOrDefaultAsync(u=>u.Account==account);
			if (user == null)
			{
				await _mySqlServerDataBaseContext._users.AddAsync(new User()
				{
					Account = account,
					Password = password
				});
				var userId = await _mySqlServerDataBaseContext.SaveChangesAsync();
				return new User()
				{
					Id = userId,
					Account = account,
					Password = password
				};
			}
			return null;
		}

		public Token? RefreshToken(string refreshToken)
		{
			var securityToken = _tokenService.Get(refreshToken);
			if (securityToken == null)
			{
				return null;
			}
			var claims = securityToken.Claims;
			if (claims == null)
			{
				return null;
			}
			var userId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			if (userId == null)
			{
				return null;
			}
			var redisToken= _redisHelper.Get($"{ERedisKey.RefreshToken}_{userId}");
			if (redisToken == null||redisToken!=refreshToken)
			{
				return null;
			}
			var user = _mySqlServerDataBaseContext._users.FirstOrDefault(u => u.Id == Convert.ToInt32(userId));
			if (user == null)
			{
				return null;
			}
			var accessToken = _tokenService.IssuaToken(_jwtOption.Value.AccessExpiration, claims.ToArray());
			var newRefreshToken = _tokenService.IssuaToken(_jwtOption.Value.RefreshExpiration, claims.ToArray());
			_redisHelper.Set($"{ERedisKey.AccessToken}_{user.Id}", accessToken);
			_redisHelper.Set($"{ERedisKey.RefreshToken}_{user.Id}", newRefreshToken);
			return new Token()
			{
				AccessToken = accessToken,
				RefreshToken = newRefreshToken
			};
		}

		public async Task<Token?> RefreshTokenAsync(string refreshToken)
		{
			var securityToken =  _tokenService.Get(refreshToken);
			if (securityToken == null)
			{
				return null;
			}
			var claims = securityToken.Claims;
			if (claims == null)
			{
				return null;
			}
			var userId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			if (userId == null)
			{
				return null;
			}
			var redisToken = await _redisHelper.GetAsync($"{ERedisKey.RefreshToken}_{userId}");
			if (redisToken == null || redisToken != refreshToken)
			{
				return null;
			}
			var user = await _mySqlServerDataBaseContext._users.FirstOrDefaultAsync(u => u.Id == Convert.ToInt32(userId));
			if (user == null)
			{
				return null;
			}
			var accessToken = _tokenService.IssuaToken(_jwtOption.Value.AccessExpiration, claims.ToArray());
			var newRefreshToken = _tokenService.IssuaToken(_jwtOption.Value.RefreshExpiration, claims.ToArray());
			await _redisHelper.SetAsync($"{ERedisKey.AccessToken}_{user.Id}", accessToken);
			await _redisHelper.SetAsync($"{ERedisKey.RefreshToken}_{user.Id}", newRefreshToken);
			return new Token()
			{
				AccessToken = accessToken,
				RefreshToken = newRefreshToken
			};
		}

	}
}
