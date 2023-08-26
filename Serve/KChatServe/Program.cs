using Config;
using JWT.Builder;
using KChatServe.Database;
using KChatServe.Filters;
using KChatServe.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using Service.Interface;
using SignalR;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;


namespace KChatServe
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers(
				option =>
				{
					option.Filters.Add<ValidateFilter>();
				});
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

				// ��� JWT �����֤
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
			builder.Services.Configure<MyConfig>(builder.Configuration.GetSection("MyConfig"));
			builder.Services.Configure<ApiBehaviorOptions>(options =>
			  options.SuppressModelStateInvalidFilter = true);
			builder.Logging.ClearProviders();
			builder.Host.UseNLog();
			new EFCore.Startup().ConfigureServices(builder.Services);
			new Service.Startup().ConfigureServices(builder.Services);
			new SignalR.Startup().ConfigureServices(builder.Services);
			var tokenService = builder.Services.BuildServiceProvider().GetService<ITokenService>();
			builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(tokenService.OnConfigration);
			builder.Services.AddCors(options =>
			{
				options.AddDefaultPolicy(builder =>
				{
				builder
				.AllowAnyMethod()
				.AllowAnyHeader()
				.SetIsOriginAllowed(_ => true)
				.AllowCredentials();
			});
			});
			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI(c =>
				{
					c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");

					// ��� JWT Token ����� Swagger UI
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
			//�����м����Ҫ�������֤����Ȩ֮ǰ���ã��������ֿ�������
			app.UseCors(); // ʹ�ÿ����м��
			app.UseAuthentication();
			app.UseAuthorization();
			app.MapHub<ChatHub>("/chathub");
			app.MapControllers();

			app.Run();
		}
	}
}