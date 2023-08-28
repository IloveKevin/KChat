using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util.Enum
{
	public enum ERedisKey
	{
		AccessToken,
		RefreshToken,
	}

	public enum EFriendRequestStatus
	{
		Waiting,
		Accepted,
		Rejected,
	}

	public enum ESignalRMessageType
	{
		FriendOnline,
		FriendOffline,
		DeleteFriend,
		NewFriend,
		NewFriendRequest,
		NewChatMessage,
		RetractChatMessage,
		ReadChatMessage,
	}

	public enum EChatMessageStatus
	{
		Waiting,
		UnSeen,
		Seen,
		Retracted
	}
}
