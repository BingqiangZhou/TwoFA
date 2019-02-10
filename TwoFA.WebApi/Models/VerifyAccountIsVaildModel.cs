using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwoFA.WebApi.Models
{
    /// <summary>
    /// 验证账号是否成功添加模型
    /// </summary>
    public class VerifyAccountIsVaildModel
    {

        /// <summary>
        /// 用户唯一标识
        /// </summary>
        public string userName { get; set; }
        /// <summary>
        /// 厂商名
        /// </summary>
        public string mName { get; set; }
        /// <summary>
        /// 秘钥
        /// </summary>
        public string key { get; set; }
    }
}