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
using Service.Implement;
using Service.Interface;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KChatServe.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class AccountContorller : ControllerBase
	{
		private readonly MySqlServerDataBaseContext _mySqlServerDataBaseContext;
		private readonly ITokenService _tokenService;
		private readonly IOptions<JWTConfigration> _jwtOption;
		private readonly IAccountService _accountService;

		public AccountContorller(MySqlServerDataBaseContext mySqlServerDataBaseContext
			, ITokenService tokenService
			, IOptions<JWTConfigration> jwtOptions
			,IAccountService accountService)
		{
			_mySqlServerDataBaseContext = mySqlServerDataBaseContext;
			_tokenService = tokenService;
			_jwtOption = jwtOptions;
			_accountService = accountService;
		}

		[HttpPost]
		public async Task<ActionResult<Token>> Login([FromBody] Login login)
		{
			var token= await _accountService.LoginAsync(login.Account, login.Password);
			if(token == null)
			{
				return BadRequest("账号或密码错误");
			}
			return Ok(token);
		}

		[HttpPost]
		public async Task<ActionResult<string>> Register([FromBody] Login login)
		{
			var user=await _accountService.RegisterAsync(login.Account, login.Password);
			if(user == null)
			{
				return BadRequest("账号已存在");
			}
			return Ok("注册成功");
		}

		[Authorize]
		[HttpGet]
		public IActionResult Test()
		{
			return Ok("你已经通过了身份验证");
		}
	}
}
