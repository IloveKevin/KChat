
namespace Service.Interface
{
	public interface IFriendService
	{
		public Task<bool> IsFriend(long userId, long friendId);
		public Task<bool> HasFriendRequest(long userId, long friendId);
		public Task<bool> AddFriendRequest(long userId, long friendId);
		public Task<bool> HandleFriendRequest(long userId,long friendId, bool accept);
		public Task<bool> DeleteFriend(long userId,long friendId);
		public Task<List<long>> GetFriendList(long userId);
		public Task<List<long>> GetFriendRequestList(long userId);
		public Task<bool> AddFriend(long userId,long friendId);
	}
}
