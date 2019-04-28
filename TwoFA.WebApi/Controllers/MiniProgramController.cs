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
        public VerifyResultViewModel ComfirmAccount(string userName, string key, string openId, string mName)
        {
            // 小程序将收到的信息发送过来一确认信息，并且添加信息（logininfo、openid）到数据库
            //验证
            User mUser = FindUserByUserName(mName);
            if (mUser == null)
            {
                return new VerifyResultViewModel { Result = false, ErrorMsg = "厂商不存在" };
            }
            //验证
            string name = EncodeUserName(mUser.Id,DecodeUserName(userName));
            User user = FindUserByUserName(userName);
            if (user == null)
            {
                return new VerifyResultViewModel { Result = false, ErrorMsg = "用户不存在" };
            }
            //验证 key是否有效添加
            bool result = VerifyManufactureNameAndKey(user.Id, mName, key);
            if(result)
            {
                //将openid更新数据库，完成账号创建
                bool res = SetOpenId(user.Id,openId);
                if (res)
                {
                    return new VerifyResultViewModel { Result = true };
                }
                else
                {
                    return new VerifyResultViewModel { Result = false,ErrorMsg="用户信息更新失败" };
                }
            }
            return new VerifyResultViewModel { Result = false ,ErrorMsg="信息不匹配"};
        }
        [HttpGet]
        public DataSynchronizationViewModel DataSynchronization(string openId)
        {
            //数据同步，通过openid（phonenumber）获取key以及mName
            var accountInfo = FindAllUserByOpenId(openId);
            if (accountInfo.Count() == 0)
            {
                return new DataSynchronizationViewModel { Result = false,ErrorMsg="无相关账号信息" };
            }
            //获取用户登录的厂商和key
            List<Account> accounts = new List<Account>();
            foreach (var item in accountInfo.AsEnumerable())
            {
                var loginInfo = FindProviderManufactureInfo(item.Id);
                if(loginInfo == null)
                {
                    continue;
                }
                foreach (var login in loginInfo.AsEnumerable())
                {
                    string uName = DecodeUserName(item.UserName);
                    accounts.Add(new Account { account = uName, key = login.ProviderKey, manufacturer = login.LoginProvider });
                }
            }
            return new DataSynchronizationViewModel { Result=true, AccountList=accounts };
        }
    }
}
