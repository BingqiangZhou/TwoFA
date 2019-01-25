using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TwoFA.WebMVC.ViewModel
{
    public class ModifyPasswordModel
    {
        public string Name { get; set; }
        [Display(Name = "当前密码")]
        [Required(ErrorMessage = "请输入密码")]
        [MinLength(6, ErrorMessage = "密码最小长度为6位")]
        [MaxLength(16, ErrorMessage = "最大长度为16位，请不要超过16位")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
        [Display(Name = "新密码")]
        [Required(ErrorMessage = "请输入密码")]
        [MinLength(6, ErrorMessage = "密码最小长度为6位")]
        [MaxLength(16, ErrorMessage = "最大长度为16位，请不要超过16位")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "确认新密码")]
        [Compare("NewPassword", ErrorMessage = "两次输入的密码不一致")]
        public string ConfirmPassword { get; set; }
    }
}