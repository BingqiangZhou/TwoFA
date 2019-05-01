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
        //[HttpPost]
        //public CreateAccountViewModel CreateAccount(AccountModel model)
        //{
        //    //查找厂商，验证厂商信息
        //    var mUser = FindUserById(model.mId);
        //    if (mUser == null)
        //    {
        //        return new CreateAccountViewModel { Result = false, ErrorMsg = "厂商不存在" };
        //    }
        //    //验证，SecurityStamp，修改密码后会改变
        //    if (mUser.SecurityStamp.Equals(model.token) == false)
        //    {
        //        return new CreateAccountViewModel { Result = false, ErrorMsg = "token不匹配" };
        //    }
        //    //用户的userName由mid+userName组成
        //    var uName = EncodeUserName(model.mId,model.userName);
        //    var user = FindUserByUserName(uName);
        //    if (user == null)
        //    {
        //        //没有找到用户，创建用户
        //        bool result = CreateUser(null,uName,"DefaultKey");
        //        if (result == false)
        //        {
        //            return new CreateAccountViewModel { Result = false,ErrorMsg="用户创建失败" };
        //        }
        //        //查询用户信息
        //        var uUser = FindUserByUserName(uName);
        //        if (uUser == null)
        //        {
        //            return new CreateAccountViewModel { Result = false ,ErrorMsg= "用户创建失败" };
        //        }
        //        user = uUser;
        //        //设置用户角色为厂商的用户
        //        result = AddRoleToOrdinaryUserById(user.Id);
        //        if (result == false)
        //        {
        //            return new CreateAccountViewModel { Result = false ,ErrorMsg="设置角色失败"};
        //        }
        //    }
        //    string twoFAToken = GenerateUserToken(user.Id);
        //    var key = GenerateCode.GenerateSHA1(model.mId+twoFAToken);
        //    //将生成的sha-1值的前十位作为重置key
        //    user.ResetKey = GenerateCode.GenerateSHA1(user.SecurityStamp + twoFAToken).Substring(0,10);
        //    bool updateRes = UpdateUser(user);
        //    if (updateRes == false)
        //    {
        //        return new CreateAccountViewModel { Result = false, ErrorMsg = "数据存储失败" };
        //    }
        //    bool res = SetUserLoginInfo(user.Id, model.mId, key);
        //    if (res == false)
        //    {
        //        return new CreateAccountViewModel { Result = false, ErrorMsg = "login数据存储失败" };
        //    }
        //    string base64String = BitmapAndBase64MutualTransformation.BitmapToBase64String(
        //        GenerateQRCodeByZxing.GenerateQRCodeToBitmap(key + "|"+ uName +"|" + mUser.UserName, 256, 256, 0));
        //    return new CreateAccountViewModel {
        //        Base64String=base64String,uName=uName,Key=key,resetKey=user.ResetKey,mName=mUser.UserName,
        //        Result=true };
        //}
        /// <summary>
        /// 验证账号是否正确添加
        /// </summary>
        /// <param name="model">参数模式</param>
        /// <returns>返回验证结果</returns>
        [HttpPost]
        public VerifyResultViewModel VerifyAccountIsVaild(AccountModel model)
        {
            //string uName = EncodeUserName(model.mId,model.userName);
            //User user = FindUserByUserName(uName);
            //if (user == null)
            //{
            //    return new VerifyResultViewModel { Result = false, ErrorMsg = "用户不存在" };
            //}
            //if (user.OpenId == null || user.OpenId.Length == 0)
            //{
            //    return new VerifyResultViewModel { Result = false, ErrorMsg = "OpenId未添加" };
            //}
            //var mUser = FindUserById(model.mId);
            //if (mUser == null || mUser.SecurityStamp.Equals(model.token) == false) 
            //{
            //    return new VerifyResultViewModel { Result = false, ErrorMsg = "厂商信息验证失败" };
            //}
            //var key = GetLoginKey(user.Id, mUser.UserName);
            //if (key != null) 
            //{
            //    return new VerifyResultViewModel { Result = true };
            //}
            return new VerifyResultViewModel { Result = false, ErrorMsg = "Login信息添加失败" };
        }
        /// <summary>
        /// 验证TwoFA账号
        /// </summary>
        /// <param name="model">验证用户参数模型</param>
        /// <returns>返回验证结果</returns>
        [HttpPost]
        public VerifyResultViewModel VerifyAccount(VerifyAccountModel model)
        {
            ////查找厂商，验证厂商信息
            //var mUser = FindUserById(model.mId);
            //if (mUser == null)
            //{
            //    return new VerifyResultViewModel { Result = false, ErrorMsg = "厂商不存在" };
            //}
            ////验证，SecurityStamp，修改密码后会改变
            //if (mUser.SecurityStamp.Equals(model.token) == false)
            //{
            //    return new VerifyResultViewModel { Result = false, ErrorMsg = "token不匹配" };
            //}
            //var uName = EncodeUserName(model.mId, DecodeUserName(model.userName));
            //var user = FindUserByUserName(uName);
            //if (user == null)
            //{
            //    return new VerifyResultViewModel { Result = false ,ErrorMsg="用户不存在"};
            //}
            //string key = GetLoginKey(user.Id, mUser.UserName);
            //if(key != null)
            //{
            //    return new VerifyResultViewModel { Result = false ,ErrorMsg="无两步验证相关信息"};
            //}
            //DateTime dt1970 = new DateTime(1970, 1, 1, 0, 0, 0).ToLocalTime();
            //DateTime current = DateTime.Now.ToLocalTime();//DateTime.UtcNow for unix timestamp
            //TimeSpan span = current - dt1970;
            //int counter = (int)Math.Floor(span.TotalMilliseconds / (30 * 1000.0));
            //double[] codeList = { GenerateCode.GenerateTwoFACode(key,counter-1),
            //GenerateCode.GenerateTwoFACode(key,counter),
            //GenerateCode.GenerateTwoFACode(key,counter+1)};
            //foreach (var item in codeList)
            //{
            //    if (item.Equals(model.code))
            //    {
            //        return new VerifyResultViewModel { Result = true };
            //    }
            //}
            return new VerifyResultViewModel { Result = false ,ErrorMsg="验证失败"};
        }
        /// <summary>
        /// 验证TwoFA重置秘钥
        /// </summary>
        /// <param name="model">验证用户参数模型</param>
        /// <returns>返回验证结果</returns>
        [HttpPost]
        public VerifyResultViewModel VerifyResetKey(VerifResetKey model)
        {
            ////查找厂商，验证厂商信息
            //var mUser = FindUserById(model.mId);
            //if (mUser == null)
            //{
            //    return new VerifyResultViewModel { Result = false, ErrorMsg = "厂商不存在" };
            //}
            ////验证，SecurityStamp，修改密码后会改变
            //if (mUser.SecurityStamp.Equals(model.token) == false)
            //{
            //    return new VerifyResultViewModel { Result = false, ErrorMsg = "token不匹配" };
            //}
            //var uName = EncodeUserName(model.mId, model.userName);
            //var user = FindUserByUserName(uName);
            //if (user == null)
            //{
            //    return new VerifyResultViewModel { Result = false, ErrorMsg = "用户不存在" };
            //}
            //if (user.ResetKey.Equals(model.resetKey))
            //{
            //    return new VerifyResultViewModel { Result = true };
            //}
            return new VerifyResultViewModel { Result = false, ErrorMsg = "验证失败" };
        }
        /// <summary>
        /// 注销TwoFA账号
        /// </summary>
        /// <param name="model">注销用户参数模型</param>
        /// <returns>返回注销结果</returns>
        [HttpPost]
        public VerifyResultViewModel CancelAccount(AccountModel model)
        {
            ////查找厂商，验证厂商信息
            //var mUser = FindUserById(model.mId);
            //if (mUser == null)
            //{
            //    return new VerifyResultViewModel { Result = false, ErrorMsg = "厂商不存在" };
            //}
            ////验证，SecurityStamp，修改密码后会改变
            //if (mUser.SecurityStamp.Equals(model.token) == false)
            //{
            //    return new VerifyResultViewModel { Result = false, ErrorMsg = "token不匹配" };
            //}
            //var uName = model.userName + "_" + model.mId.Replace('-', '_');
            //var user = FindUserByUserName(uName);
            //if (user == null)
            //{
            //    return new VerifyResultViewModel { Result = false, ErrorMsg = "用户不存在" };
            //}
            //DeleteUserLogin(user.Id);
            //var res = IsOrdinaryUser(user.Id);
            ////为普通用户
            //if (res == true)
            //{
            //    var result = DeleteUser(user);
            //    if (result == false)
            //    {
            //        return new VerifyResultViewModel { Result = false, ErrorMsg = "删除用户信息时出错" };
            //    }
            //}
            //else //厂商用户
            //{
            //    user.OpenId = null;
            //    var result = UpdateUser(user);
            //    if (result == false)
            //    {
            //        return new VerifyResultViewModel { Result = false, ErrorMsg = "删除用户信息时出错" };
            //    }
            //}
            return new VerifyResultViewModel { Result = true };
        }
    }
}
