using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using TwoFA.Utils.ToolsClass;
using TwoFA.WebApi.ViewModels;
using TwoFA.WebMVC.Models.Infrastructure;
using TwoFA.WebMVC.Models.Model;

namespace TwoFA.WebApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MiniProgramController : TwoFAApiController
    {
        public string GetUserInfo(string code)
        {
            string url = string.Format("https://api.weixin.qq.com/sns/jscode2session?appid=wxdeb9b0c22d1602d0&secret=8d2b3bafac0859123911cf1807f020ca&js_code={0}&grant_type=authorization_code", code);
            string res = HttpTool.Visit(url);
            return res;
        }
        [HttpGet]
        public async Task<VerifyResultViewModel> ComfirmAccount(string userName, string key, string openId, string mName)
        {
            // 小程序将收到的信息发送过来一确认信息，并且添加信息（logininfo、openid）到数据库
            //验证
            var mUser = await UserManager.FindByNameAsync(mName);
            if (mUser == null)
            {
                return new VerifyResultViewModel { Result = false, ErrorMsg = "厂商不存在" };
            }
            //验证
            var user = await UserManager.FindByNameAsync(userName);
            if (user == null)
            {
                return new VerifyResultViewModel { Result = false, ErrorMsg = "用户不存在" };
            }
            //验证 key是否有效添加
            var loginInfos = await UserManager.GetLoginsAsync(user.Id);
            foreach (var item in loginInfos)
            {
                //验证 厂商和秘钥
                if (item.LoginProvider.Equals(mName) && item.ProviderKey.Equals(key))
                {
                    //将openid更新数据库，完成账号创建
                    user.OpenID = openId;
                    var res = await UserManager.UpdateAsync(user);
                    if (res.Succeeded)
                    {
                        return new VerifyResultViewModel { Result = true };
                    }
                    else
                    {
                        return new VerifyResultViewModel { Result = false,ErrorMsg="用户信息更新失败" };
                    }
                }
            }
            return new VerifyResultViewModel { Result = false ,ErrorMsg="信息不匹配"};
        }
        [HttpGet]
        public async Task<DataSynchronizationViewModel> DataSynchronization(string openId)
        {
            //数据同步，通过openid（phonenumber）获取key以及mName
            List<Account> accounts = new List<Account>();
            var accountInfo = UserManager.Users.AsEnumerable().Where(u => u.OpenID == openId).Select(m=> new {Id= m.Id,Name = m.UserName });
            if (accountInfo.Count() == 0)
            {
                return new DataSynchronizationViewModel { Result = false,ErrorMsg="无相关账号信息" };
            }
            foreach (var item in accountInfo.AsEnumerable())
            {
                var loginInfo = await UserManager.GetLoginsAsync(item.Id);
                if(loginInfo == null)
                {
                    continue;
                }
                foreach (var login in loginInfo.AsEnumerable())
                {
                    string uName = item.Name;
                    //判断是否是厂商
                    if (item.Name.Contains("_") == true)
                    {
                        uName = uName.Split('_')[0];
                    }
                    accounts.Add(new Account { account = uName, key = login.ProviderKey, manufacturer = login.LoginProvider });
                }
            }
            return new DataSynchronizationViewModel { Result=true, AccountList=accounts };
        }
    }
}
