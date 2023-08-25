using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Model.JWT
{
	public class LoginToken
	{
		[JsonPropertyName("iss")]
		public string Audience { get; set; }
		[JsonPropertyName("exp")]
		public long Expiration { get; set; }
		[JsonPropertyName("aud")]
		public long AccountId { get; set; }
	}
}
