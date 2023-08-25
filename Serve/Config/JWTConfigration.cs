using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config
{
	public class JWTConfigration
	{
		public string Issuer { get; set; }
		public string Secret { get; set; }
		public string Audience { get; set; }
		public long AccessExpiration { get; set; }
		public long RefreshExpiration { get; set; }
	}
}
