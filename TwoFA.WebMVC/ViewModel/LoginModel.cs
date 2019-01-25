using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TwoFA.WebMVC.ViewModel
{
    public class LoginModel
    {
        [Display(Name = "邮箱")]
        [Required(ErrorMessage = "请输入邮箱号")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$", ErrorMessage = "请输入正确的邮箱。")]
        public string Email { get; set; }
        [Display(Name = "密码")]
        [Required(ErrorMessage = "请输入密码")]
        [MinLength(6, ErrorMessage = "密码最小长度为6位")]
        [MaxLength(16, ErrorMessage = "最大长度为16位，请不要超过16位")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}