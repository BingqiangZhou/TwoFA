using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwoFA.WebApi.ViewModels
{
    public class DataSynchronizationViewModel
    {
        public List<Account> AccountList { get; set; }
        /// <summary>
        /// 结果状态码，true表示验证码验证成功
        /// </summary>
        public bool Result { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg { get; set; }
    }

    public class Account
    {
        public string key { get; set; }
        public string account { get; set; }
        public string manufacturer { get; set; }
    }
}