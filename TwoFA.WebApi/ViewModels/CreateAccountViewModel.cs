using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwoFA.WebApi.ViewModels
{
    public class CreateAccountViewModel
    {
        /// <summary>
        /// 二维码base64字符串形式，包含Key+厂商ID信息
        /// </summary>
        public string Base64String { get; set; }
        /// <summary>
        /// 厂商名
        /// </summary>
        public string mName { get; set; }
        /// <summary>
        /// 用户唯一标识
        /// </summary>
        public string uName { get; set; }
        /// <summary>
        /// 生成TwoFA验证码的秘钥
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 结果状态码，true表示返回正确的信息
        /// </summary>
        public bool Result { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg { get; set; }
    }
}