using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwoFA.WebApi.Models
{
    /// <summary>
    /// 验证用户数据模型
    /// </summary>
    public class VerifyAccountModel
    {
        /// <summary>
        /// 厂商提供该用户唯一标识
        /// </summary>
        public string userName { get; set; }
        /// <summary>
        /// 厂商id
        /// </summary>
        public string mId { get; set; }
        /// <summary>
        /// 厂商token
        /// </summary>
        public string token { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public double code { get; set; }
    }
}