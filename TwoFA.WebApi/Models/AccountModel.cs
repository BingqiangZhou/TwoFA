using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwoFA.WebApi.Models
{
    /// <summary>
    /// 创建用户数据模型
    /// </summary>
    public class AccountModel
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
    }
}