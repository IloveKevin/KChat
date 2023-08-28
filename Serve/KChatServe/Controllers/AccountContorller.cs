using Config;
using KChatServe.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Model.HttpModel.Request;
using Model.HttpModel.Response;
using Model.JWT;
using Service.Implement;
using Service.Interface;
using SignalR;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Util.SignalR;

namespace KChatServe.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class AccountContorller : ControllerBase
	{
		private readonly IAccountService _accountService;
		private readonly IHubContext<ChatHub> _chatHub;

		public AccountContorller(IAccountService accountService,IHubContext<ChatHub> chatHub)
		{
			_accountService = accountService;
			_chatHub = chatHub;
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
		[HttpPost]
		public async Task<ActionResult<Token>> RefreshToken([FromBody] string refreshToken)
		{
			var token = await _accountService.RefreshTokenAsync(refreshToken);
			if(token == null)
			{
				return BadRequest("刷新失败");
			}
			return Ok(token);
		}

		[Authorize]
		[HttpPost]
		public async Task<ActionResult<bool>> UpdateNickName([FromBody] string nickName)
		{
			var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
			var result = await _accountService.UpdateNickName(userId, nickName);
			if(!result)
			{
				return BadRequest("修改失败");
			}
			return Ok(true);
		}

		[Authorize]
		[HttpGet]
		public async Task<ActionResult<UserInfo>> GetUsersInfo([FromQuery] List<long> usersId)
		{
			return Ok(await _accountService.GetUserInfo(usersId));
		}
	}
}
