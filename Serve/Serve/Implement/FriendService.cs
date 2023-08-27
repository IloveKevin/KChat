using EFCore.Entity;
using KChatServe.Database;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.Enum;

namespace Service.Implement
{
	public class FriendService : IFriendService
	{
		private readonly MySqlServerDataBaseContext _mySqlServerDataBaseContext;

		public FriendService(MySqlServerDataBaseContext mySqlServerDataBaseContext)
		{
			_mySqlServerDataBaseContext = mySqlServerDataBaseContext;
		}


		public async Task<bool> AddFriend(long userId, long friendId)
		{
			var isFriend = await IsFriend(userId, friendId);
			if (isFriend)
			{
				return false;
			}
			var friend = new Friend()
			{
				UserId = userId,
				FriendId = friendId,
			};
			await _mySqlServerDataBaseContext._friends.AddAsync(friend);
			await _mySqlServerDataBaseContext.SaveChangesAsync();
			return true;
		}

		public async Task<bool> AddFriendRequest(long userId, long friendId)
		{
			var hasFriendRequest = await HasFriendRequest(userId, friendId);
		    if(hasFriendRequest)
			{
			    return false;
		    }
			var friendRequest = new FriendRequest()
			{
				UserId = userId,
				FriendId = friendId,
				Status = Util.Enum.EFriendRequestStatus.Waiting,
			};
			await _mySqlServerDataBaseContext._friendRequests.AddAsync(friendRequest);
			await _mySqlServerDataBaseContext.SaveChangesAsync();
			return true;
		}

		public async Task<bool> DeleteFriend(long userId, long friendId)
		{
			var friend = await _mySqlServerDataBaseContext._friends.FirstOrDefaultAsync(x => (x.UserId == userId && x.FriendId == friendId) || (x.UserId == friendId && x.FriendId == userId));
			if (friend == null)
			{
				return false;
			}
			_mySqlServerDataBaseContext._friends.Remove(friend);
			await _mySqlServerDataBaseContext.SaveChangesAsync();
			return true;
		}

		public async Task<List<long>> GetFriendList(long userId)
		{
			return await _mySqlServerDataBaseContext._friends.Where(x => x.UserId == userId||x.FriendId==userId).Select(x=>userId==x.UserId?x.FriendId:x.UserId).ToListAsync();
		}

		public async Task<List<long>> GetFriendRequestList(long userId)
		{
			return await _mySqlServerDataBaseContext._friendRequests.Where(x => x.FriendId == userId&&x.Status==EFriendRequestStatus.Waiting).Select(x=>x.UserId).ToListAsync();
		}

		public async Task<bool> HandleFriendRequest(long userId, long friendId, bool accept)
		{
			var friendRequest = await _mySqlServerDataBaseContext._friendRequests.FirstOrDefaultAsync(x => x.UserId == friendId && x.FriendId == userId&&x.Status==EFriendRequestStatus.Waiting);
			if (friendRequest == null)
			{
				return false;
			}
			if (accept)
			{
				await AddFriend(userId, friendId);
			}
			friendRequest.Status = accept ? EFriendRequestStatus.Accepted : EFriendRequestStatus.Rejected;
			await _mySqlServerDataBaseContext.SaveChangesAsync();
			return true;
		}

		public async Task<bool> IsFriend(long userId, long friendId)
		{
			return await _mySqlServerDataBaseContext._friends.AnyAsync(x => (x.UserId == userId && x.FriendId == friendId)||(x.UserId==friendId&&x.FriendId==userId));
		}
		
		public async Task<bool> HasFriendRequest(long userId,long friendId)
		{
			return await _mySqlServerDataBaseContext._friendRequests.AnyAsync(x => (x.UserId == userId && x.FriendId == friendId)||(x.UserId==friendId&&x.FriendId==userId));
		}
	}
}
