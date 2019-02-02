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
using TwoFA.WebApi.ViewModels;
using TwoFA.WebMVC.Models.Infrastructure;
using TwoFA.WebMVC.Models.Model;

namespace TwoFA.WebApi.Controllers
{
    public class TwoFAController : TwoFAApiController
    {
        /// <summary>
        /// 开通TwoFA账号
        /// </summary>
        /// <param name="userName">厂商提供该用户唯一id</param>
        /// <param name="mid">厂商openId</param>
        /// <param name="token">厂商token</param>
        /// <returns>返回二维码图片base64字符串，以及手动添加账号需输入的Token</returns>
        [HttpGet]
        public async Task<CreateAccountViewModel> CreateAccount(string userName,
            string mid="TwoFA",string token="token")
        {
            //用户的userName由mid+userName组成
            User user;
            if (mid.Equals("TwoFA"))
            {
                 user = await UserManager.FindByNameAsync(userName);
            }
            else
            {
                user = await UserManager.FindByNameAsync(userName + "_" + mid.Replace('-', '_'));
            }
            if (user == null)
            {
                if (mid.Equals("TwoFA"))
                {
                    return new CreateAccountViewModel { Result = false,ErrorMsg="厂商不存在" };
                }
                //没有找到用户，创建用户
                var uName = userName + "_" + mid.Replace('-','_') ;
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
            string mName = mid;
            if (mid.Equals("TwoFA") == false)
            {
                //验证，厂商用户是否存在
                User mUser = await UserManager.FindByIdAsync(mid);
                if (mUser == null)
                {
                    return new CreateAccountViewModel { Result = false , ErrorMsg = "厂商不存在" };
                }
                //验证，SecurityStamp，修改密码后会改变
                if (mUser.SecurityStamp.Equals(token) == false)
                {
                    return new CreateAccountViewModel { Result = false , ErrorMsg = "token不匹配" };
                }
                mName = mUser.UserName;
            }
            string twoFAToken = UserManager.GenerateUserToken("TwoFA KEY", user.Id);
            var key = GenerateCode.GenerateTwoFAKey(mid, twoFAToken);
            IdentityResult res = await UserManager.AddLoginAsync(user.Id, new UserLoginInfo(mName, key));
            if (res.Succeeded == false)
            {
                return new CreateAccountViewModel { Result = false, ErrorMsg = "数据存储失败" };
            }
            string base64String = BitmapAndBase64MutualTransformation.BitmapToBase64String(
                GenerateQRCodeByZxing.GenerateQRCodeToBitmap(key + "_"+userName+"_" + mName, 256, 256, 0));
            return new CreateAccountViewModel {  Base64String=base64String,Key=key,mName=mName,Result=true };
        }

        [HttpPost]
        public async Task<VerifyResultViewModel> VerifyCode(double code, string userName,string mId,string token)
        {
            var uName = userName + "_" + mId.Replace('-', '_');
            var user = await UserManager.FindByNameAsync(uName);
            if (user == null)
            {
                return new VerifyResultViewModel { Result = false ,ErrorMsg="用户不存在"};
            }
            var mUser = await UserManager.FindByIdAsync(mId);
            if (mUser == null || mUser.SecurityStamp.Equals(token) == false)
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
                if (item.Equals(code))
                {
                    return new VerifyResultViewModel { Result = true };
                }
            }
            return new VerifyResultViewModel { Result = false ,ErrorMsg="验证失败"};
        }
    }
}
