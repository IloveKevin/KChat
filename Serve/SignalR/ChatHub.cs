using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Service.Interface;
using System.Linq;
using Util.Enum;

namespace SignalR
{
	[Authorize]
	public class ChatHub : Hub
	{
		private readonly IFriendService _friendService;

		public ChatHub(IFriendService friendService)
		{
			_friendService = friendService;
		}
		public override async Task OnConnectedAsync()
		{
			var userIdentity = Context.UserIdentifier;
			if(userIdentity==null)
			{
				return;
			}
			var userId = long.Parse(userIdentity);
			IReadOnlyList<string> friendList = (await _friendService.GetFriendList(userId)).Select(x => x.ToString()).ToList();
			await Clients.Clients(friendList).SendAsync(ESignalRMessageType.FriendOnline.ToString(), userId);
			await base.OnConnectedAsync();
		}
		public async Task SendMessage(string user, string message)
		{
			await Clients.All.SendAsync("ReceiveMessage", user, message);
		}

		public override async Task OnDisconnectedAsync(Exception? exception)
		{
			var userIdentity = Context.UserIdentifier;
			if (userIdentity == null)
			{
				return;
			}
			var userId = long.Parse(userIdentity);
			IReadOnlyList<string> friendList = (await _friendService.GetFriendList(userId)).Select(x => x.ToString()).ToList();
			await Clients.Clients(friendList).SendAsync(ESignalRMessageType.FriendOffline.ToString(), userId);
			await base.OnDisconnectedAsync(exception);
		}
	}
}