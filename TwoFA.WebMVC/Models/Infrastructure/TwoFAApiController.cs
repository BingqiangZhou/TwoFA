using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using TwoFA.Utils.ToolsClass;
using TwoFA.WebMVC.Models.Context;
using TwoFA.WebMVC.Models.Model;

namespace TwoFA.WebMVC.Models.Infrastructure
{
    public class TwoFAApiController : ApiController
    {
        #region Identity相关上下文，UserManager，RoleManager，AuthManager
        //获取用户管理上下文
        public static TwoFAUserManager UserManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().GetUserManager<TwoFAUserManager>();
            }
        }
        //获取角色管理上下文
        public static TwoFARoleManager RoleManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().GetUserManager<TwoFARoleManager>();
            }
        }
        //获取认证管理上下文
        public static IAuthenticationManager AuthManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }

        private TwoFADbContext db
        {
            get
            {
                return new TwoFADbContext();
            }
        }
        #endregion

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="email">用户邮箱</param>
        /// <param name="name">用户名</param>
        /// <param name="password">用户密码</param>
        /// <returns>操作成功返回true,操作失败返回false</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public bool CreateUser(string name)
        {
            var result = UserManager.Create(new User {  UserName = name });
            return result.Succeeded;
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="user">用户对象</param>
        /// <returns>操作成功返回true,操作失败返回false</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public bool UpdateUser(User user)
        {
            var result = UserManager.Update(user);
            return result.Succeeded;
        }


        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public bool DeleteUser(User user)
        {
            var loginInfos = UserManager.GetLogins(user.Id);
            if (loginInfos != null)
            {
                foreach (var item in loginInfos)
                {
                    var res = UserManager.RemoveLogin(user.Id, item);
                    if (res.Succeeded == false)
                    {
                        return false;
                    }
                }
            }
            var result = UserManager.Delete(user);
            return result.Succeeded;
        }

        /// <summary>
        /// 删除用户登录信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public bool DeleteUserLogin(string id,string mId)
        {
            var loginInfos = UserManager.GetLogins(id);
            if (loginInfos != null)
            {
                foreach (var item in loginInfos)
                {
                    if (item.LoginProvider.Equals(mId))
                    {
                        var res = UserManager.RemoveLogin(id, item);
                        return res.Succeeded;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 获取秘钥
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="mid">厂商id</param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public string GetLoginKey(string id,string mid)
        {
            var loginInfos = UserManager.GetLogins(id);
            foreach (var item in loginInfos)
            {
                if (item.LoginProvider.Equals(mid)
                    && item.ProviderKey != null && item.ProviderKey.Length == 40)
                {
                    return item.ProviderKey;
                }
            }
            return null;
        }

        /// <summary>
        /// 设置用户登录信息
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="mName">登录厂商</param>
        /// <param name="key">两步验证秘钥</param>
        /// <returns>操作成功返回true,操作失败返回false</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public bool SetUserLoginInfo(string id, string mId, string twoFAKey)
        {
            //找到声明"ReturnUrl"，删除声明
            var userLogins = UserManager.GetLogins(id);
            foreach (var login in userLogins)
            {
                if (login.LoginProvider.Equals(mId))
                {
                    var removeResult = UserManager.RemoveLogin(id, login);
                    if (removeResult.Succeeded == false)
                    {
                        return false;
                    }
                    break;
                }
            }
            //添加新的声明
            var addResult = UserManager.AddLogin(id, new UserLoginInfo(mId, twoFAKey));
            return addResult.Succeeded;
        }

        /// <summary>
        /// 添加角色到厂商
        /// </summary>
        /// <param name="id"></param>
        /// <returns>操作成功返回true,操作失败返回false</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public bool AddRoleToManufactruerById(string id)
        {
            var result = UserManager.AddToRole(id, "M");
            return result.Succeeded;
        }

        /// <summary>
        /// 添加角色到普通用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns>操作成功返回true,操作失败返回false</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public bool AddRoleToOrdinaryUserById(string id)
        {
            var result = UserManager.AddToRole(id, "O");
            return result.Succeeded;
        }

        /// <summary>
        /// 通过邮箱查找用户
        /// </summary>
        /// <param name="email">邮箱号</param>
        /// <returns>查找到用户返回User对象，未找到返回null</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public User FindUserByEmail(string email)
        {
            User user = null;
            user = UserManager.FindByEmail(email);
            return user;
        }

        /// <summary>
        /// 通过用户名查找用户
        /// </summary>
        /// <param name="name">用户名</param>
        /// <returns>查找到用户返回User对象，未找到返回null</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public string FindUserIdByUserName(string name)
        {
            string user = null;
            user = db.Users.Where(x=>x.Name == name).Select(x=>x.Id).FirstOrDefault();
            return user;
        }

        /// <summary>
        /// 通过用户名查找用户
        /// </summary>
        /// <param name="name">用户名</param>
        /// <returns>查找到用户返回User对象，未找到返回null</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public string FindUserIdByNameWithmId(string name, string mId)
        {
            string user = null;
            user = db.Users.Where(x => x.Name == name && x.Manufacturer == mId).Select(x => x.Id).FirstOrDefault();
            return user;
        }

        /// <summary>
        /// 通过id查找用户
        /// </summary>
        /// <param name="id">用户名</param>
        /// <returns>查找到用户返回User对象，未找到返回null</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public User FindUserById(string id)
        {
            User user = null;
            user = UserManager.FindById(id);
            return user;
        }

        /// <summary>
        /// 获取所有用户信息通过openId
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public IEnumerable<User> FindAllUserByOpenId(string openId)
        {
            return UserManager.Users.AsEnumerable().Where(u => u.OpenId == openId);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IEnumerable<UserLoginInfo> FindProviderManufactureInfo(string id)
        {
            return UserManager.GetLogins(id);
        }
        /// <summary>
        /// 验证账号，通过邮箱和密码
        /// </summary>
        /// <param name="email">邮箱号</param>
        /// <param name="password">密码</param>
        /// <returns>验证成功返回User对象，验证失败返回null</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public User VerifyAccountByEmailAndPassword(string email, string password)
        {
            User user = FindUserByEmail(email);
            if (user == null)
                return null;
            user = UserManager.Find(user.UserName, password);
            return user;
        }

        /// <summary>
        /// 获取Token,登录时生成，登录成功控制器验证
        /// </summary>
        /// <param name="id">用户id</param>
        /// <returns>返回token</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public string GetTokenById(string id)
        {
            return UserManager.GenerateEmailConfirmationToken(id);
        }

        /// <summary>
        /// 解码用户名
        /// </summary>
        /// <param name="user">User对象</param>
        /// <returns>返回用户名</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public string DecodeUserName(string userName)
        {
            return userName.Split('_')[0];
        }

        /// <summary>
        /// 编码用户名
        /// </summary>
        /// <param name="appId">厂商id</param>
        /// <param name="name">用户名</param>
        /// <returns>返回用户名</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public string EncodeUserName(string appId, string name)
        {
            //厂商Id
            string appID = appId.Replace('-', '_');
            //用户名
            string userName = name + "_" + appID;
            return userName;
        }

        /// <summary>
        /// 通过id获取ReturnURL
        /// </summary>
        /// <param name="id"></param>
        /// <returns>返回ReturnURL，未找到则返回null</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public string GetReturnURLById(string id)
        {
            string url = null;
            var claims = UserManager.GetClaims(id);
            foreach (var claim in claims)
            {
                if (claim.Type.Equals("ReturnUrl"))
                {
                    url = claim.Value;
                    break;
                }
            }
            return url;
        }

        /// <summary>
        /// 设置OpenId
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="openId"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public bool SetOpenId(string id, string openId)
        {
            User user = FindUserById(id);
            user.OpenId = openId;
            var res = UserManager.Update(user);
            return res.Succeeded;
        }

        /// <summary>
        /// 验证厂商和秘钥
        /// </summary>
        /// <param name="uId">用户id</param>
        /// <param name="mName">厂商名</param>
        /// <param name="key">秘钥</param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public bool VerifyManufactureNameAndKey(string uId,string mId,string key)
        {
            var loginInfos = UserManager.GetLogins(uId);
            foreach (var item in loginInfos)
            {
                //验证 厂商和秘钥
                if (item.LoginProvider.Equals(mId) && item.ProviderKey.Equals(key))
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 设置ReturnURL
        /// </summary>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <returns>操作成功返回true,操作失败返回false</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public bool SetReturnURLById(string id, string url)
        {
            //找到声明"ReturnUrl"，删除声明
            var claims = UserManager.GetClaims(id);
            foreach (var claim in claims)
            {
                if (claim.Type.Equals("ReturnUrl"))
                {
                    var removeResult = UserManager.RemoveClaim(id, claim);
                    if (removeResult.Succeeded == false)
                    {
                        return false;
                    }
                    break;
                }
            }
            //添加新的声明
            var addResult = UserManager.AddClaim(id, new Claim("ReturnUrl", url));
            return addResult.Succeeded;
        }

        /// <summary>
        /// 生成用于用户的token
        /// </summary>
        /// <param name="id"></param>
        /// <returns>正确生成token返回token,否则返回null</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public string GenerateUserToken(string id)
        {
            return UserManager.GenerateUserToken("TwoFA KEY", id);
        }

        /// <summary>
        /// 生成重置密码token
        /// </summary>
        /// <param name="id"></param>
        /// <returns>正确生成token返回token,否则返回null</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public string GeneratePasswordResetTokenById(string id)
        {
            return UserManager.GeneratePasswordResetToken(id);
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="token"></param>
        /// <param name="password">新密码</param>
        /// <returns>操作成功返回true,操作失败返回false</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public bool ResetPassword(string id, string token, string password)
        {
            var result = UserManager.ResetPassword(id, token, password);
            return result.Succeeded;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        /// <returns>操作成功返回true,操作失败返回false</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public bool ChangePassword(string id, string oldPassword, string newPassword)
        {
            var result = UserManager.ChangePassword(id, oldPassword, newPassword);
            return result.Succeeded;
        }

        /// <summary>
        /// 获取重置密码token
        /// </summary>
        /// <param name="id"></param>
        /// <returns>成功获取到token，返回token，否则返回null</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public string GetPasswordResetTokenById(string id)
        {
            string token = null;
            var claims = UserManager.GetClaims(id);
            foreach (var claim in claims)
            {
                if (claim.Type.Equals("PasswordResetToken"))
                {
                    token = HttpUtility.UrlDecode(claim.Value);
                    break;
                }
            }
            return token;
        }

        /// <summary>
        /// 设置重置密码token
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <returns>返回设置token操作的结果，成功返回true，失败返回false</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public bool SetPasswordResetTokenById(string id, string token)
        {
            //找到声明"PasswordResetToken"，删除声明
            var claims = UserManager.GetClaims(id);
            foreach (var claim in claims)
            {
                if (claim.Type.Equals("PasswordResetToken"))
                {
                    var removeResult = UserManager.RemoveClaim(id, claim);
                    if (removeResult.Succeeded == false)
                    {
                        return false;
                    }
                    break;
                }
            }
            //添加新的声明
            var addResult = UserManager.AddClaim(id, new Claim("PasswordResetToken", token));
            return addResult.Succeeded;
        }

        /// <summary>
        /// 验证token是否与生成的token匹配
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="token"></param>
        /// <returns>返回验证token操作的结果，匹配成功返回true，失败返回false</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public bool VerifyPasswordResetToken(string id, string token)
        {
            string tokenTemp = null;
            Claim claimTmp = null;
            var claims = UserManager.GetClaims(id);
            foreach (var claim in claims)
            {
                if (claim.Type.Equals("PasswordResetToken"))
                {
                    tokenTemp = HttpUtility.UrlDecode(claim.Value);
                    claimTmp = claim;
                    break;
                }
            }
            //匹配则删除token
            if (token != null && tokenTemp != null && token.Equals(tokenTemp))
            {
                var result = UserManager.RemoveClaim(id, claimTmp);
                return result.Succeeded;
            }
            return false;
        }
    }
}
