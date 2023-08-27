using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util.Redis
{
	public interface IRedisHelper
	{
		bool Set(string key, string value);
		Task<bool> SetAsync(string key, string value);
		string? Get(string key);
		Task<string?> GetAsync(string key);
		bool Delete(string key);
		Task<bool> DeleteAsync(string key);
	}
}
