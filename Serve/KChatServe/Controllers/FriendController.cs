using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Service.Interface;
using SignalR;
using System.Security.Claims;
using Util.Enum;

namespace KChatServe.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class FriendController : ControllerBase
	{
		private readonly IFriendService _friendService;
		private readonly IHubContext<ChatHub> _hubContext;

		public FriendController(IFriendService friendService,IHubContext<ChatHub> hubContext)
		{
			_friendService = friendService;
			_hubContext = hubContext;
		}

		[Authorize]
		[HttpPost]
		public async Task<ActionResult<bool>> AddFriend([FromQuery] long friendId)
		{
			var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
			var result = await _friendService.AddFriend(userId, friendId);
			if(result == false)
			{
				return BadRequest("添加好友失败");
			}
			else
			{
				await _hubContext.Clients.User(friendId.ToString()).SendAsync(ESignalRMessageType.NewFriendRequest.ToString(), userId);
				return Ok("添加好友成功");
			}
		}

		[Authorize]
		[HttpPost]
		public async Task<ActionResult<bool>> HandleFriendRequest([FromQuery] long friendId, [FromQuery] bool accept)
		{
			var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
			var result = await _friendService.HandleFriendRequest(userId, friendId,accept);
			if (result == false)
			{
				return BadRequest("接受好友失败");
			}
			else
			{
				await _hubContext.Clients.User(friendId.ToString()).SendAsync(ESignalRMessageType.NewFriend.ToString(), userId);
				return Ok("接受好友成功");
			}
		}

		[Authorize]
		[HttpGet]
		public async Task<ActionResult<List<long>>> GetFriendList()
		{
			var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
			var result = await _friendService.GetFriendList(userId);
			return Ok(result);
		}

		[Authorize]
		[HttpGet]
		public async Task<ActionResult<List<long>>> GetFriendRequestList()
		{
			var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
			var result = await _friendService.GetFriendRequestList(userId);
			return Ok(result);
		}

		[Authorize]
		[HttpPost]
		public async Task<ActionResult<bool>> DeleteFriend([FromQuery] long friendId)
		{
			var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
			var result = await _friendService.DeleteFriend(userId, friendId);
			if (result == false)
			{
				return BadRequest("删除好友失败");
			}
			else
			{
				await _hubContext.Clients.User(friendId.ToString()).SendAsync(ESignalRMessageType.DeleteFriend.ToString(), userId);
				return Ok("删除好友成功");
			}
		}
	}
}
