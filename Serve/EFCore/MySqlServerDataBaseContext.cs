using Config;
using EFCore.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KChatServe.Database
{
	public class MySqlServerDataBaseContext : DbContext
	{
		public DbSet<User> _users { get; set; }
		private readonly IOptions<SqlServerConfigration>? _sqlserverOption;
		private readonly ILogger<MySqlServerDataBaseContext>? _logger;
		private readonly string? _connectionString;
		/// <summary>
		/// 这个无参的构造函数仅仅只有在执行数据库迁移时使用
		/// </summary>
		public MySqlServerDataBaseContext()
		{
			_connectionString = "server=.;uid=sa;pwd=123456;database=kChat;TrustServerCertificate=true";
		}
		public MySqlServerDataBaseContext(IOptions<SqlServerConfigration> sqlserverOption,ILogger<MySqlServerDataBaseContext> logger)
		{
			_sqlserverOption = sqlserverOption;
			_logger = logger;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			var connectionString = _connectionString!=null? _connectionString:_sqlserverOption?.Value.ConnectionString;
			optionsBuilder.UseSqlServer(connectionString);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(MySqlServerDataBaseContext).Assembly);
		}
	}
}
