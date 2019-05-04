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
        public ResultInfo ComfirmAccount(string userName, string key, string openId, string mName)
        {
            // 小程序将收到的信息发送过来一确认信息，并且添加信息（logininfo、openid）到数据库
            //验证
            string id = FindUserIdByUserName(mName);
            User mUser = FindUserById(id);
            if (mUser == null)
            {
                return new ResultInfo { result = false, errorMsg = "厂商不存在" };
            }
            //验证
            string uId = FindUserIdByNameWithmId(userName,mUser.Id);
            User user = FindUserById(uId);
            if (user == null)
            {
                return new ResultInfo { result = false, errorMsg = "用户不存在" };
            }
            //验证 key是否有效添加
            bool result = VerifyManufactureNameAndKey(user.Id, mUser.Id, key);
            if(result)
            {
                //将openid更新数据库，完成账号创建
                bool res = SetOpenId(user.Id,openId);
                if (res)
                {
                    return new ResultInfo { result = true };
                }
                else
                {
                    return new ResultInfo { result = false,errorMsg="用户信息更新失败" };
                }
            }
            return new ResultInfo { result = false ,errorMsg="信息不匹配"};
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
                if (item.OpenId != null && item.OpenId.Length > 0)
                {
                    var loginInfo = FindProviderManufactureInfo(item.Id);
                    if (loginInfo == null)
                    {
                        continue;
                    }
                    foreach (var login in loginInfo.AsEnumerable())
                    {
                        var mUser = FindUserById(login.LoginProvider);
                        accounts.Add(new Account { account = item.Name, key = login.ProviderKey, manufacturer = mUser.Name });
                    }
                }
            }
            return new DataSynchronizationViewModel { Result=true, AccountList=accounts };
        }
    }
}
