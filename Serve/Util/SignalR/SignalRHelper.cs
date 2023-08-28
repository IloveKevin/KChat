using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util.SignalR
{
	public class SignalRHelper
	{
		public static List<long> S_OnlineUser=new List<long>();
		public static void AddOnlineUser(long userId)
		{
			if (!S_OnlineUser.Contains(userId))
			{
				S_OnlineUser.Add(userId);
			}
		}

		public static void RemoveOnlineUser(long userId)
		{
			if (S_OnlineUser.Contains(userId))
			{
				S_OnlineUser.Remove(userId);
			}
		}

		public static bool IsOnline(long userId)
		{
			return S_OnlineUser.Contains(userId);
		}
	}
}
