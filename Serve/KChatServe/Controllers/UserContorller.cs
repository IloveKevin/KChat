using Config;
using KChatServe.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Model.HttpModel.Request;
using Model.HttpModel.Response;
using Model.JWT;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Util.JWT;

namespace KChatServe.Controllers
{
    [ApiController]
	[Route("api/[controller]/[action]")]
	public class UserContorller : ControllerBase
	{
		private readonly MySqlServerDataBaseContext _mySqlServerDataBaseContext;
		private readonly IJWTHelper _jWTHelper;
		private readonly IOptions<JWTConfigration> _jwtOption;

		public UserContorller(MySqlServerDataBaseContext mySqlServerDataBaseContext
			,IJWTHelper jWTHelper
			,IOptions<JWTConfigration> jwtOptions)
		{
			_mySqlServerDataBaseContext = mySqlServerDataBaseContext;
			_jWTHelper = jWTHelper;
			_jwtOption = jwtOptions;
		}

		[HttpPost]
		public async Task<ActionResult<Token>> Login([FromBody]Login login)
		{
			var user = await _mySqlServerDataBaseContext._users.FirstOrDefaultAsync(u => u.Account == login.Account && u.Password == login.Password);
			if (user == null)
			{
				return BadRequest("账号或密码错误");
			}
			var claims = new[]
			{
				new Claim("ID",user.Id.ToString()),
			};
			var accessToken = _jWTHelper.IssuaToken(_jwtOption.Value.AccessExpiration,claims);
			var refreshToken = _jWTHelper.IssuaToken(_jwtOption.Value.RefreshExpiration,claims);
			return Ok(new Token()
			{
				AccessToken = accessToken,
				RefreshToken = refreshToken
			});
		}

		[HttpPost]
		public async Task<ActionResult<string>> Register([FromBody] Login login)
		{
			var user=_mySqlServerDataBaseContext._users.FirstOrDefault(u=>u.Account==login.Account);
			if(user == null)
			{
				await _mySqlServerDataBaseContext._users.AddAsync(new EFCore.Entity.User()
				{
					Account = login.Account,
					Password = login.Password
				});
				await _mySqlServerDataBaseContext.SaveChangesAsync();
				return Ok("恭喜你，注册成功");
			}
			return BadRequest("oh! no! 你注册失败了");
		}
	}
}
