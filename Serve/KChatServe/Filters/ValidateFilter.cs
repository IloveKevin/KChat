using Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Model.Result;

namespace KChatServe.Filters
{
	public class ValidateFilter : ActionFilterAttribute
	{
		private readonly IOptions<MyConfig> _option;

		public ValidateFilter(IOptions<MyConfig> option)
		{
			_option = option;
		}
		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			if (!context.ModelState.IsValid)
			{
				string? message = "";
				if (_option.Value.ErrorMessageOnlyOne)
				{
					message = context.ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage;
				}
				else
				{
					foreach (var item in context.ModelState.Values)
					{
						foreach (var error in item.Errors)
						{
							message += error.ErrorMessage + "\n";
						}
					}
				}
				context.Result = new ObjectResult(new BaseResultModel(422, message, false))
				{
					StatusCode = 422
				};
			}
			else
			{
				await next();
			}
		}
	}
}
