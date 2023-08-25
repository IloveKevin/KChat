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

namespace Service.Implement
{
	public class AccountService : IAccountService
	{
		private readonly MySqlServerDataBaseContext _mySqlServerDataBaseContext;
		private readonly ITokenService _tokenService;
		private readonly IOptions<JWTConfigration> _jwtOption;

		public AccountService(MySqlServerDataBaseContext mySqlServerDataBaseContext
			,ITokenService tokenService
			,IOptions<JWTConfigration> jwtOption)
		{
			_mySqlServerDataBaseContext = mySqlServerDataBaseContext;
			_tokenService = tokenService;
			_jwtOption = jwtOption;
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
				new Claim("ID",user.Id.ToString()),
			};
			var accessToken = _tokenService.IssuaToken(_jwtOption.Value.AccessExpiration,claims);
			var refreshToken = _tokenService.IssuaToken(_jwtOption.Value.RefreshExpiration, claims);
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
				new Claim("ID",user.Id.ToString()),
			};
			var accessToken = _tokenService.IssuaToken(_jwtOption.Value.AccessExpiration, claims);
			var refreshToken = _tokenService.IssuaToken(_jwtOption.Value.RefreshExpiration, claims);
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

	}
}
