using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCore.Entity
{
	public class User
	{
		public long Id { get; set; }
		public string Account { get; set; }
		public string Password { get; set; }
	}
}
