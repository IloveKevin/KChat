using Config;
using JWT.Builder;
using KChatServe.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;
using Util.JWT;

namespace KChatServe
{
    public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

				// 添加 JWT 身份验证
				var securityScheme = new OpenApiSecurityScheme
				{
					Name = "Authorization",
					Description = "JWT Authorization header using the Bearer scheme",
					In = ParameterLocation.Header,
					Type = SecuritySchemeType.Http,
					Scheme = "bearer",
					Reference = new OpenApiReference
					{
						Type = ReferenceType.SecurityScheme,
						Id = JwtBearerDefaults.AuthenticationScheme
					}
				};

				c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);
				c.AddSecurityRequirement(new OpenApiSecurityRequirement
		{
			{ securityScheme, new List<string>() }
		});
			});
			builder.Services.Configure<SqlServerConfigration>(builder.Configuration.GetSection("SqlServerConfigration"));
			builder.Services.Configure<JWTConfigration>(builder.Configuration.GetSection("JWTConfigration"));
			builder.Services.AddDbContext<MySqlServerDataBaseContext>();
			builder.Logging.ClearProviders();
			builder.Host.UseNLog();
			builder.Services.AddScoped<IJWTHelper, JWTHelper>();
			var jwtHelper = builder.Services.BuildServiceProvider().GetService<IJWTHelper>();
			builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(jwtHelper.OnConfigration);
			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI(c =>
				{
					c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");

					// 添加 JWT Token 输入框到 Swagger UI
					c.DocumentTitle = "Your API";
					c.DocExpansion(DocExpansion.None);
					c.DefaultModelExpandDepth(2);
					c.DefaultModelsExpandDepth(0);
					c.DefaultModelRendering(ModelRendering.Example);
					c.DisplayRequestDuration();
					c.EnableDeepLinking();
					c.EnableFilter();
					c.ShowExtensions();
					c.EnableValidator();
					c.SupportedSubmitMethods(SubmitMethod.Get, SubmitMethod.Post, SubmitMethod.Put, SubmitMethod.Delete);
				});
			}

			app.UseHttpsRedirection();
			app.UseAuthentication();
			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}