using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Service.Interface;
using System.Linq;
using Util.Enum;
using Util.SignalR;

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
			if (userIdentity == null)
			{
				return;
			}
			var userId = long.Parse(userIdentity);
			IReadOnlyList<string> friendList = (await _friendService.GetFriendList(userId)).Select(x => x.ToString()).ToList();
			await Clients.Clients(friendList).SendAsync(ESignalRMessageType.FriendOnline.ToString(), userId);
			SignalRHelper.AddOnlineUser(userId);
			await base.OnConnectedAsync();
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
			SignalRHelper.RemoveOnlineUser(userId);
			await base.OnDisconnectedAsync(exception);
		}
	}
}