using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.Enum;

namespace EFCore.Entity
{
    public class FriendRequest
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long FriendId { get; set; }
		public EFriendRequestStatus Status { get; set; }
	}
}
