using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.Enum;

namespace EFCore.Entity
{
	public class Message
	{
		public long Id { get; set; }
		public long SenderId { get; set; }
		public long ReceiverId { get; set; }
		public string Content { get; set; }
		public DateTime SendTime { get; set; }
		public EChatMessageStatus Status { get; set; }
	}
}
