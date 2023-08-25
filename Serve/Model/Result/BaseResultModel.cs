using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Result
{
	public class BaseResultModel
	{
		public int Code { get; set; }
		public object? Data { get; set; }
		public string? Message { get; set; }
		public string Time { get; set; } 
		public BaseResultModel(int code,object? data,bool correct = true)
		{
			Code = code;
			Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			if (correct)
			{
				Message = "请求成功";
				Data = data;
			}
			else
			{
				Message = data?.ToString()??"";
			}
		}
	}
}
