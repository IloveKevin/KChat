using Config;
using EFCore.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Service.Interface;
using SignalR;
using System.Security.Claims;
using Util.Enum;
using Util.SignalR;

namespace KChatServe.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class MessageController : ControllerBase
	{
		private readonly IMessageService _messageService;
		private readonly IHubContext<ChatHub> _hubContext;

		public MessageController(IMessageService messageService, IHubContext<ChatHub> hubContext, IOptions<ChatConfigration> chatOption)
		{
			_messageService = messageService;
			_hubContext = hubContext;
		}

		[Authorize]
		[HttpPost]
		public async Task<ActionResult<Message>> AddMessage([FromBody] long friendId, [FromBody] string content)
		{
			var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
			var message = await _messageService.AddMessage(userId, friendId, content);
			if (SignalRHelper.IsOnline(userId))
			{
				await _hubContext.Clients.User(userId.ToString()).SendAsync(ESignalRMessageType.NewChatMessage.ToString(), message);
				await _messageService.SendMessage(message.Id);
			}
			return Ok(message);
		}

		[Authorize]
		[HttpGet]
		public async Task<ActionResult<List<Message>>> GetChatHistory([FromQuery] long friendId, [FromQuery] long start, [FromQuery] int size)
		{
			var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
			var messages = await _messageService.GetHistoryMessages(userId, friendId, start, size);
			return Ok(messages);
		}

		[Authorize]
		[HttpGet]
		public async Task<ActionResult<List<Message>>> GetUnReadMessages()
		{
			var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
			var messages = await _messageService.GetUnReadMessages(userId);
			messages.ForEach(async x => await _messageService.SendMessage(x.Id));
			return Ok(messages);
		}

		[Authorize]
		[HttpPost]
		public async Task<ActionResult<bool>> RetractMessage([FromBody] long messageId)
		{
			var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
			var result = await _messageService.RetractMessage(userId, messageId);
			if (result != null)
			{
				await _hubContext.Clients.User(result.ReceiverId.ToString()).SendAsync(ESignalRMessageType.RetractChatMessage.ToString(), messageId);
				return BadRequest(false);
			}
			return Ok(true);
		}

		[Authorize]
		[HttpPost]
		public ActionResult ReadMessages([FromBody] List<long> messageIds)
		{
			var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
			messageIds.ForEach(async x =>
			{
				var result = await _messageService.ReadMessage(userId, x);
				if (result != null) { await _hubContext.Clients.User(result.ReceiverId.ToString()).SendAsync(ESignalRMessageType.ReadChatMessage.ToString(), result.Id); }
			});
			return Ok();
		}
	}
}
