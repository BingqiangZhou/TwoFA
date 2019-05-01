using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TwoFA.WebMVC.ViewModel
{
    public class VerifyModel
    {
        [Display(Name = "验证码")]
        //[Required(ErrorMessage = "请输入验证码")]
        [RegularExpression(@"\d{6}", ErrorMessage = "请输入正确的六位验证码")]
        public double code { get; set; }
        [Display(Name = "重置码")]
        //[Required(ErrorMessage = "请输入验证码")]
        [RegularExpression(@"([a-z]|[A-Z]|[0-9]){12}", ErrorMessage = "请输入正确的重置码")]
        public string resetCode { get; set; }
        public string userName { get; set; }
        public string mId { get; set; }
        public string token { get; set; }
        public string ReturnURL { get; set; }
        public string accessToken { get; set; }
        public string __RequestVerificationToken { get; set; }
    }
}