﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwoFA.WebApi.ViewModels
{
    /// <summary>
    /// 基础结果响应模型
    /// </summary>
    public class ResultInfo
    {
        /// <summary>
        /// 结果状态码，true表示验证码验证成功
        /// </summary>
        public bool result { get; set; }
        /// <summary>
        /// 错误信息，验证成功时为null
        /// </summary>
        public string errorMsg { get; set; }
    }
}