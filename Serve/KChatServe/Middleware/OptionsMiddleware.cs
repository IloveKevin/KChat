namespace KChatServe.Middleware
{
	public class OptionsMiddleware
	{
		private readonly RequestDelegate _next;

		public OptionsMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext context)
		{
			if (context.Request.Method == "OPTIONS")
			{
				// Handle the OPTIONS request
				context.Response.Headers.Add("Access-Control-Allow-Origin", "http://127.0.0.1:5500");
				//添加x-requested-with和Authorization
				context.Response.Headers.Add("Access-Control-Allow-Headers", "x-requested-with,Authorization");
				context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE");
				context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
				context.Response.StatusCode = 200;
				return;
			}

			await _next(context);
		}
	}

	public static class OptionsMiddlewareExtensions
	{
		public static IApplicationBuilder UseOptionsMiddleware(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<OptionsMiddleware>();
		}
	}
}
