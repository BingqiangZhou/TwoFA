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
        public async Task<CreateAccountViewModel> CreateAccount(CreateAccountModel model)
        {
            //用户的userName由mid+userName组成
            User user;
            if (model.mid.Equals("TwoFA"))
            {
                //mid参数值为"TwoFA"，表示为给企业用户创建两步验证
                 user = await UserManager.FindByNameAsync(model.userName);
            }
            else
            {
                user = await UserManager.FindByNameAsync(model.userName + "_" + model.mid.Replace('-', '_'));
            }
            if (user == null)
            {
                if (model.mid.Equals("TwoFA"))
                {
                    return new CreateAccountViewModel { Result = false,ErrorMsg="厂商不存在" };
                }
                //没有找到用户，创建用户
                var uName = model.userName + "_" + model.mid.Replace('-','_') ;
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
            var key = GenerateCode.GenerateTwoFAKey(model.mid, twoFAToken);
            IdentityResult res = await UserManager.AddLoginAsync(user.Id, new UserLoginInfo(mName, key));
            if (res.Succeeded == false)
            {
                return new CreateAccountViewModel { Result = false, ErrorMsg = "数据存储失败" };
            }
            string base64String = BitmapAndBase64MutualTransformation.BitmapToBase64String(
                GenerateQRCodeByZxing.GenerateQRCodeToBitmap(key + "_"+ model.userName +"_" + mName, 256, 256, 0));
            return new CreateAccountViewModel {  Base64String=base64String,Key=key,mName=mName,Result=true };
        }

        /// <summary>
        /// 验证TwoFA账号
        /// </summary>
        /// <param name="model">验证用户参数模型</param>
        /// <returns>返回验证结果</returns>
        [HttpPost]
        public async Task<VerifyResultViewModel> VerifyAccount(VerifyAccountModel model)
        {
            var uName = model.userName + "_" + model.mId.Replace('-', '_');
            var user = await UserManager.FindByNameAsync(uName);
            if (user == null)
            {
                return new VerifyResultViewModel { Result = false ,ErrorMsg="用户不存在"};
            }
            var mUser = await UserManager.FindByIdAsync(model.mId);
            if (mUser == null || mUser.SecurityStamp.Equals(model.token) == false)
            {
                return new VerifyResultViewModel { Result = false ,ErrorMsg="厂商不存在"};
            }
            var loginInfoList = await UserManager.GetLoginsAsync(user.Id);
            string key="none";
            foreach (var item in loginInfoList)
            {
                if (item.LoginProvider.Equals(mUser.UserName))
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
    }
}
