using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Exhibition.Models
{
    public class User
    {
        
        public int Id { get; set; }
        [Display(Name = "姓名")]
        [Required(ErrorMessage ="请输入姓名！")]
        public string Name { get; set; }
        [Display(Name = "密码")]
        [Required(ErrorMessage = "请输入密码！")]
        public string Password { get; set; }
    }
}