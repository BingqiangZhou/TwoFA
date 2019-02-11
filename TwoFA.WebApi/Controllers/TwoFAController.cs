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
    public class TwoFAController : TwoFAApiController
    {
        /// <summary>
        /// 开通TwoFA账号
        /// </summary>
        /// <param name="model">创建用户参数模型</param>
        /// <returns>返回二维码图片base64字符串，以及手动添加账号需输入的Token</returns>
        [HttpPost]
        public async Task<CreateAccountViewModel> CreateAccount(AccountModel model)
        {
            //用户的userName由mid+userName组成
            var uName = model.userName;
            if (model.mid.Equals("TwoFA") == false)
            {
                uName = model.userName + "_" + model.mid.Replace('-', '_');
            }
            var user = await UserManager.FindByNameAsync(uName);
            if (user == null)
            {
                if (model.mid.Equals("TwoFA"))
                {
                    return new CreateAccountViewModel { Result = false,ErrorMsg="厂商不存在" };
                }
                //没有找到用户，创建用户
                IdentityResult result = await UserManager.CreateAsync(new User { UserName = uName });
                if (result.Succeeded == false)
                {
                    return new CreateAccountViewModel { Result = false,ErrorMsg="用户创建失败" };
                }
                //查询用户信息
                var uUser = await UserManager.FindByNameAsync(uName);
                if (uUser == null)
                {
                    return new CreateAccountViewModel { Result = false ,ErrorMsg="用户不存在"};
                }
                user = uUser;
                //设置用户角色为厂商的用户
                result = await UserManager.AddToRoleAsync(user.Id, "U");
                if (result.Succeeded == false)
                {
                    return new CreateAccountViewModel { Result = false ,ErrorMsg="设置角色失败"};
                }
            }
            //判断开通账号是否为厂商
            string mName = model.mid;
            if (model.mid.Equals("TwoFA") == false)
            {
                //验证，厂商用户是否存在
                User mUser = await UserManager.FindByIdAsync(model.mid);
                if (mUser == null)
                {
                    return new CreateAccountViewModel { Result = false , ErrorMsg = "厂商不存在" };
                }
                //验证，SecurityStamp，修改密码后会改变
                if (mUser.SecurityStamp.Equals(model.token) == false)
                {
                    return new CreateAccountViewModel { Result = false , ErrorMsg = "token不匹配" };
                }
                mName = mUser.UserName;
            }
            string twoFAToken = UserManager.GenerateUserToken("TwoFA KEY", user.Id);
            var key = GenerateCode.GenerateSHA1(model.mid+twoFAToken);
            //将生成的sha-1值的前十位作为重置key
            user.ResetKey = GenerateCode.GenerateSHA1(user.SecurityStamp + twoFAToken).Substring(0,10);
            IdentityResult updateRes = await UserManager.UpdateAsync(user);
            if (updateRes.Succeeded == false)
            {
                return new CreateAccountViewModel { Result = false, ErrorMsg = "数据存储失败" };
            }
            IdentityResult res = await UserManager.AddLoginAsync(user.Id, new UserLoginInfo(mName, key));
            if (res.Succeeded == false)
            {
                return new CreateAccountViewModel { Result = false, ErrorMsg = "login数据存储失败" };
            }
            string base64String = BitmapAndBase64MutualTransformation.BitmapToBase64String(
                GenerateQRCodeByZxing.GenerateQRCodeToBitmap(key + "_"+ model.userName +"_" + mName, 256, 256, 0));
            return new CreateAccountViewModel {
                Base64String =base64String,uName=uName,Key=key,resetKey=user.ResetKey,mName=mName,Result=true };
        }
        /// <summary>
        /// 验证账号是否正确添加
        /// </summary>
        /// <param name="model">参数模式</param>
        /// <returns>返回验证结果</returns>
        public async Task<VerifyResultViewModel> VerifyAccountIsVaild(VerifyAccountIsVaildModel model)
        {
            var user = await UserManager.FindByNameAsync(model.userName);
            if (user == null)
            {
                return new VerifyResultViewModel { Result = false, ErrorMsg = "用户不存在" };
            }
            if (user.OpenID.Length == 0)
            {
                return new VerifyResultViewModel { Result = false, ErrorMsg = "OpenId添加失败" };
            }
            var loginInfos = await UserManager.GetLoginsAsync(user.Id);
            if (loginInfos == null)
            {
                return new VerifyResultViewModel { Result = false, ErrorMsg = "Key添加失败" };
            }
            foreach (var item in loginInfos)
            {
                if (item.ProviderKey.Equals(model.key)&&item.LoginProvider.Equals(model.mName))
                {
                    return new VerifyResultViewModel { Result = true };
                }
            }
            return new VerifyResultViewModel { Result = false, ErrorMsg = "Login信息添加失败" };
        }
        /// <summary>
        /// 验证TwoFA账号
        /// </summary>
        /// <param name="model">验证用户参数模型</param>
        /// <returns>返回验证结果</returns>
        [HttpPost]
        public async Task<VerifyResultViewModel> VerifyAccount(VerifyAccountModel model)
        {
            var uName = model.userName;
            User mUser = null;
            if (model.mId.Equals("TwoFA") == false)
            {
                uName = model.userName + "_" + model.mId.Replace('-', '_');
                mUser = await UserManager.FindByIdAsync(model.mId);
                if (mUser == null || mUser.SecurityStamp.Equals(model.token) == false)
                {
                    return new VerifyResultViewModel { Result = false, ErrorMsg = "厂商不存在" };
                }
            }
            var user = await UserManager.FindByNameAsync(uName);
            if (user == null)
            {
                return new VerifyResultViewModel { Result = false ,ErrorMsg="用户不存在"};
            }
            var mName = "TwoFA";
            if (mUser != null)
            {
                mName = mUser.UserName;
            }
            var loginInfoList = await UserManager.GetLoginsAsync(user.Id);
            string key="none";
            foreach (var item in loginInfoList)
            {
                if (item.LoginProvider.Equals(mName))
                {
                    key = item.ProviderKey;
                }
            }
            if(key.Equals("none") == true)
            {
                return new VerifyResultViewModel { Result = false ,ErrorMsg="无两步验证相关信息"};
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
                    return new VerifyResultViewModel { Result = true };
                }
            }
            return new VerifyResultViewModel { Result = false ,ErrorMsg="验证失败"};
        }
        /// <summary>
        /// 验证TwoFA重置秘钥
        /// </summary>
        /// <param name="model">验证用户参数模型</param>
        /// <returns>返回验证结果</returns>
        [HttpPost]
        public async Task<VerifyResultViewModel> VerifyResetKey(VerifResetKey model)
        {
            var uName = model.userName;
            User mUser = null;
            if (model.mId.Equals("TwoFA") == false)
            {
                uName = model.userName + "_" + model.mId.Replace('-', '_');
                mUser = await UserManager.FindByIdAsync(model.mId);
                if (mUser == null || mUser.SecurityStamp.Equals(model.token) == false)
                {
                    return new VerifyResultViewModel { Result = false, ErrorMsg = "厂商不存在" };
                }
            }
            var user = await UserManager.FindByNameAsync(uName);
            if (user == null)
            {
                return new VerifyResultViewModel { Result = false, ErrorMsg = "用户不存在" };
            }
            if (user.ResetKey.Equals(model.resetKey))
            {
                return new VerifyResultViewModel { Result = true};
            }
            return new VerifyResultViewModel { Result = false, ErrorMsg = "验证失败" };
        }
        /// <summary>
        /// 注销TwoFA账号
        /// </summary>
        /// <param name="model">注销用户参数模型</param>
        /// <returns>返回注销结果</returns>
        [HttpPost]
        public async Task<VerifyResultViewModel> CancelAccount(AccountModel model)
        {
            //用户的userName由mid+userName组成
            var uName = model.userName;
            if (model.mid.Equals("TwoFA") == false)
            {
                uName = model.userName + "_" + model.mid.Replace('-', '_');
            }
            //验证信息
            if (model.mid.Equals("TwoFA") == false)
            {
                //验证，厂商用户是否存在
                User mUser = await UserManager.FindByIdAsync(model.mid);
                if (mUser == null)
                {
                    return new VerifyResultViewModel { Result = false, ErrorMsg = "厂商不存在" };
                }
                //验证，SecurityStamp，修改密码后会改变
                if (mUser.SecurityStamp.Equals(model.token) == false)
                {
                    return new VerifyResultViewModel { Result = false, ErrorMsg = "token不匹配" };
                }
            }
            var user = await UserManager.FindByNameAsync(uName);
            if (user == null)
            {
                return new VerifyResultViewModel { Result = false, ErrorMsg = "用户不存在" };
            }
            var loginInfos = await UserManager.GetLoginsAsync(user.Id);
            if (loginInfos != null)
            {
                foreach (var item in loginInfos)
                {
                    var result = await UserManager.RemoveLoginAsync(user.Id, item);
                    if (result.Succeeded == false)
                    {
                        return new VerifyResultViewModel { Result = false, ErrorMsg = "删除信息时出错" };
                    }
                }
            }
            var res = await UserManager.IsInRoleAsync(user.Id, "U");
            //为普通用户
            if (res == true)
            {
                var result = await UserManager.DeleteAsync(user);
                if (result.Succeeded == false)
                {
                    return new VerifyResultViewModel { Result = false, ErrorMsg = "删除用户信息时出错" };
                }
            }
            else //厂商用户
            {
                user.OpenID = "";
                var result = await UserManager.UpdateAsync(user);
                if (result.Succeeded == false)
                {
                    return new VerifyResultViewModel { Result = false, ErrorMsg = "删除用户信息时出错" };
                }
            }
            return new VerifyResultViewModel { Result = true };
        }
    }
}
