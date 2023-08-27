using Config;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util.Redis
{
	public class RedisHelper : IRedisHelper
	{
		private readonly IOptions<RedisConfigration> _redisOption;
		private readonly ConnectionMultiplexer _redis;

		public RedisHelper(IOptions<RedisConfigration> redisOption)
		{
			_redisOption = redisOption;
			_redis = ConnectionMultiplexer.Connect(_redisOption.Value.Server);
		}

		public async Task<bool> SetAsync(string key, string value)
		{
			var db = _redis.GetDatabase();
			return await db.StringSetAsync(key, value);
		}

		public async Task<string?> GetAsync(string key)
		{
			var db = _redis.GetDatabase();
			return await db.StringGetAsync(key);
		}

		public async Task<bool> DeleteAsync(string key)
		{
			var db = _redis.GetDatabase();
			return await db.KeyDeleteAsync(key);
		}

		public bool Set(string key, string value)
		{
			var db = _redis.GetDatabase();
			return db.StringSet(key, value);
		}

		public string? Get(string key)
		{
			var db = _redis.GetDatabase();
			return db.StringGet(key);
		}

		public bool Delete(string key)
		{
		    var db = _redis.GetDatabase();
			return db.KeyDelete(key);
		}
	}
}
