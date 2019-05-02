using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using TwoFA.Utils.ToolsClass;
using TwoFA.WebApi.Models;
using TwoFA.WebApi.ViewModels;
using TwoFA.WebMVC.Models.Infrastructure;
using TwoFA.WebMVC.Models.Model;

namespace TwoFA.WebApi.Controllers
{
    /// <summary>
    /// 两步验证开放API
    /// </summary>
    public class TwoFAServiceController : TwoFAApiController
    {
        /// <summary>
        /// 开启两步验证服务
        /// </summary>
        /// <param name = "model" > 参数模型 </param>
        /// <returns> 返回二维码图片base64字符串，以及生成验证码的key</returns>
        [HttpGet]
        public OpenServiceViewModel Open(BaseModel model)
        {
            User mUser = FindUserById(model.mId);
            if (mUser == null)
            {
                return new OpenServiceViewModel { result = false, errorMsg = "非法访问！     001" };
            }
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("user", model.user);          //传入用户唯一标识，这里省略查询用户数据步骤，传入用户id
            dict.Add("mId", model.mId);                 //这里传企业两步验证账号id
            dict.Add("signatureKey", mUser.SecurityStamp);            //这里传入企业两步验证token
            string timestamp = Singature.GetTimeStamp();//获取当前时间戳
            dict.Add("timestamp", timestamp);           //传入当前时间戳，当前时间到1970-1-1的秒数
            string sign = Singature.GetSignature(dict); //获取到sign
            if (sign.Equals(model.sign))
            {
                return new OpenServiceViewModel { result = false, errorMsg = "签名无效！" };
            }
            string id = FindUserIdByNameWithmId(model.user, model.mId);
            User user = FindUserById(id);
            if (user == null)
            {
                bool res = CreateUser(model.user);
                if (res == false)
                {
                    return new OpenServiceViewModel { result = false, errorMsg = "未知错误，请重试！ 001" };
                }
            }
            user = FindUserById(id);
            string twoFAToken = GenerateUserToken(mUser.Id);
            string twoFAKey = GenerateCode.GenerateSHA1(user.Id + twoFAToken);
            //将生成的sha-1值的前12位作为重置key
            string resetKey = GenerateCode.GenerateSHA1(user.SecurityStamp + twoFAToken).Substring(0, 12);
            user.ResetKey = resetKey;
            bool updateRes = UpdateUser(user);
            if (updateRes == false)
            {
                return new OpenServiceViewModel { result = false, errorMsg = "未知错误，请重试！     002" };
            }
            bool loginRes = SetUserLoginInfo(user.Id, model.mId, twoFAKey);
            if (loginRes == false)
            {
                new OpenServiceViewModel { result = false, errorMsg = "未知错误，请重试！     003" };
            }
            string base64String = BitmapAndBase64MutualTransformation.BitmapToBase64String(
                            GenerateQRCodeByZxing.GenerateQRCodeToBitmap(twoFAKey + "|" + user.Name + "|" + mUser.Name, 256, 256, 0));
            if (base64String == null)
            {
                new OpenServiceViewModel { result = false, errorMsg = "未知错误，请重试！     004" };
            }
            return new OpenServiceViewModel { base64String = base64String, resetKey = user.ResetKey, result = true };
        }
        /// <summary>
        /// 验证账号是否正确添加
        /// </summary>
        /// <param name="model">参数模型</param>
        /// <returns>返回验证结果</returns>
        [HttpGet]
        public ResultInfo VerifyIsOpened(BaseModel model)
        {
            User mUser = FindUserById(model.mId);
            if (mUser == null)
            {
                return new ResultInfo { result = false, errorMsg = "非法访问！     001" };
            }
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("user", model.user);          //传入用户唯一标识，这里省略查询用户数据步骤，传入用户id
            dict.Add("mId", model.mId);                 //这里传企业两步验证账号id
            dict.Add("signatureKey", mUser.SecurityStamp);            //这里传入企业两步验证token
            string timestamp = Singature.GetTimeStamp();//获取当前时间戳
            dict.Add("timestamp", timestamp);           //传入当前时间戳，当前时间到1970-1-1的秒数
            string sign = Singature.GetSignature(dict); //获取到sign
            if (sign.Equals(model.sign))
            {
                return new ResultInfo { result = false, errorMsg = "签名无效！" };
            }
            string id = FindUserIdByNameWithmId(model.user, model.mId);
            User user = FindUserById(id);
            if (user == null || user.OpenId == null || user.OpenId.Length == 0)
            {
                return new ResultInfo { result = false, errorMsg = "请您正确添加账号再验证！" };
            }
            var key = GetLoginKey(user.Id, mUser.UserName);
            if (key == null || key.Length == 0)
            {
                return new ResultInfo { result = false, errorMsg = "未知错误，请重试！     001" };
            }
            return new ResultInfo { result = true};
        }
        /// <summary>
        /// 验证两步验证验证码
        /// </summary>
        /// <param name="model">参数模型</param>
        /// <returns>返回验证结果</returns>
        [HttpGet]
        public ResultInfo Verify(VerifyModel model)
        {
            User mUser = FindUserById(model.mId);
            if (mUser == null)
            {
                return new ResultInfo { result = false, errorMsg = "非法访问！     001" };
            }
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("user", model.user);          //传入用户唯一标识，这里省略查询用户数据步骤，传入用户id
            dict.Add("mId", model.mId);                 //这里传企业两步验证账号id
            dict.Add("signatureKey", mUser.SecurityStamp);            //这里传入企业两步验证token
            dict.Add("code", model.code.ToString());          //这里传入企业两步验证token
            string timestamp = Singature.GetTimeStamp();//获取当前时间戳
            dict.Add("timestamp", timestamp);           //传入当前时间戳，当前时间到1970-1-1的秒数
            string sign = Singature.GetSignature(dict); //获取到sign
            if (sign.Equals(model.sign))
            {
                return new ResultInfo { result = false, errorMsg = "签名无效！" };
            }
            string id = FindUserIdByNameWithmId(model.user, model.mId);
            User user = FindUserById(id);
            if (user == null || user.OpenId == null || user.OpenId.Length == 0)
            {
                return new ResultInfo { result = false, errorMsg = "该账号未开通两步验证服务！" };
            }
            string key = GetLoginKey(user.Id, mUser.Id);
            if (key == null)
            {
                return new ResultInfo { result = false, errorMsg = "无两步验证相关信息" };
            }
            DateTime dt1970 = new DateTime(1970, 1, 1, 0, 0, 0).ToLocalTime();
            DateTime current = DateTime.Now.ToLocalTime();//DateTime.UtcNow for unix timestamp
            TimeSpan span = current - dt1970;
            int counter = (int)Math.Floor(span.TotalMilliseconds / (30 * 1000.0));
            double[] codeList = { GenerateCode.GenerateTwoFACode(key,counter-1),
            GenerateCode.GenerateTwoFACode(key,counter),
            GenerateCode.GenerateTwoFACode(key,counter+1)};
            foreach (var item in codeList)
            {
                if (item.Equals(model.code))
                {
                    return new ResultInfo { result = true };
                }
            }
            return new ResultInfo { result = false ,errorMsg="验证码不正确！"};
        }
        /// <summary>
        /// 关闭两步验证服务
        /// </summary>
        /// <param name="model">参数模型</param>
        /// <returns>返回验证结果</returns>
        [HttpGet]
        public ResultInfo Close(CloseModel model)
        {
            User mUser = FindUserById(model.mId);
            if (mUser == null)
            {
                return new ResultInfo { result = false, errorMsg = "非法访问！     001" };
            }
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("user", model.user);          //传入用户唯一标识，这里省略查询用户数据步骤，传入用户id
            dict.Add("mId", model.mId);                 //这里传企业两步验证账号id
            dict.Add("signatureKey", mUser.SecurityStamp);            //这里传入企业两步验证token
            dict.Add("resetKey", model.resetKey);          //这里传入企业两步验证token
            string timestamp = Singature.GetTimeStamp();//获取当前时间戳
            dict.Add("timestamp", timestamp);           //传入当前时间戳，当前时间到1970-1-1的秒数
            string sign = Singature.GetSignature(dict); //获取到sign
            if (sign.Equals(model.sign))
            {
                return new ResultInfo { result = false, errorMsg = "签名无效！" };
            }
            string id = FindUserIdByNameWithmId(model.user, model.mId);
            User user = FindUserById(id);
            if (user == null || user.OpenId == null || user.OpenId.Length == 0)
            {
                return new ResultInfo { result = false, errorMsg = "该账号未开通两步验证服务！" };
            }
            if (user.ResetKey.Equals(model.resetKey) == false)
            {
                return new ResultInfo { result = false, errorMsg = "验证码错误！" };
            }
            user.OpenId = null;
            bool res= DeleteUserLogin(user.Id,mUser.Id);
            if (res == false)
            {
                return new ResultInfo { result = false, errorMsg = "未知错误！     001" };
            }
            return new ResultInfo { result = true };
        }
        /// <summary>
        /// 获取重置码
        /// </summary>
        /// <param name="model">参数模型</param>
        /// <returns>返回操作结果</returns>
        [HttpGet]
        public ResetKeyModel GetResetKey(BaseModel model)
        {
            User mUser = FindUserById(model.mId);
            if (mUser == null)
            {
                return new ResetKeyModel { result = false, errorMsg = "非法访问！     001" };
            }
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("user", model.user);          //传入用户唯一标识，这里省略查询用户数据步骤，传入用户id
            dict.Add("mId", model.mId);                 //这里传企业两步验证账号id
            dict.Add("signatureKey", mUser.SecurityStamp);            //这里传入企业两步验证token
            string timestamp = Singature.GetTimeStamp();//获取当前时间戳
            dict.Add("timestamp", timestamp);           //传入当前时间戳，当前时间到1970-1-1的秒数
            string sign = Singature.GetSignature(dict); //获取到sign
            if (sign.Equals(model.sign))
            {
                return new ResetKeyModel { result = false, errorMsg = "签名无效！" };
            }
            string id = FindUserIdByNameWithmId(model.user, model.mId);
            User user = FindUserById(id);
            if (user == null || user.OpenId == null || user.OpenId.Length == 0)
            {
                return new ResetKeyModel { result = false, errorMsg = "该账号未开通两步验证服务！" };
            }
            return new ResetKeyModel { result = true, resetKey = user.ResetKey };
        }
    }
}
