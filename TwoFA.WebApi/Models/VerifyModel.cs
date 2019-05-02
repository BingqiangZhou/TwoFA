using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwoFA.WebApi.Models
{
    /// <summary>
    /// 两步验证验证参数模型
    /// </summary>
    public class VerifyModel
    {
        /// <summary>
        /// 厂商提供该用户唯一标识
        /// </summary>
        public string user { get; set; }
        /// <summary>
        /// 厂商id
        /// </summary>
        public string mId { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string sign { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        public string timestamp { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public int code { get; set; }
    }
}