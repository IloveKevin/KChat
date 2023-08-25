using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Model.HttpModel.Request
{
    public class Login
    {
        [Required(ErrorMessage = "账号不可以为空")]
        [StringLength(12, MinimumLength = 6, ErrorMessage = "账号长度必须在6-12位之间")]
        public string Account { get; set; }
        [Required(ErrorMessage = "密码不可以为空")]
        [StringLength(12, MinimumLength = 6, ErrorMessage = "密码长度必须在6-12位之间")]
        public string Password { get; set; }
    }
}
