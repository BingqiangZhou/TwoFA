using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TwoFA.WebMVC.Models.Context;
using TwoFA.WebMVC.Models.Model;

namespace TwoFA.WebMVC.Models.Infrastructure
{
    public class TwoFAMVCController : Controller
    {

        #region Identity相关上下文，UserManager，RoleManager，AuthManager
        //获取用户管理上下文
        private TwoFAUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<TwoFAUserManager>();
            }
        }
        //获取角色管理上下文
        private TwoFARoleManager RoleManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<TwoFARoleManager>();
            }
        }
        //获取授权管理
        private IAuthenticationManager AuthManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
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
        /// 判断是否有用户已经登录
        /// </summary>
        /// <returns>有用户已登录则返回User对象，没有则返回null</returns>
        public User HaveUserLogined()
        {
            var userName = HttpContext.User.Identity.Name;
            if (userName == null)
            {
                return null;
            }
            User user = null;
            user = UserManager.FindByName(userName);
            return user;
        }

        /// <summary>
        /// 获取秘钥
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="mid">厂商id</param>
        /// <returns></returns>
        public string GetLoginKey(string id, string mid)
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
        /// 创建用户
        /// </summary>
        /// <param name="email">用户邮箱</param>
        /// <param name="name">用户名</param>
        /// <param name="password">用户密码</param>
        /// <param name="mId">厂商id</param>
        /// <returns>操作成功返回true,操作失败返回false</returns>
        public bool CreateUser(string email,string name,string password,string mId)
        {
            User user = new User { Email = email, Name = name,Manufacturer=mId};
            //UserName 无实际意义
            user.UserName = Guid.NewGuid().ToString().Replace("-","");//生成guid，填充UserName
            var result = UserManager.Create(user,password);
            return result.Succeeded;
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="email">用户邮箱</param>
        /// <param name="name">用户名</param>
        /// <param name="password">用户密码</param>
        /// <param name="mId">厂商id</param>
        /// <returns>操作成功返回true,操作失败返回false</returns>
        public bool CreateCustomUser(string name,string mId)
        {
            User user = new User {  Name = name, Manufacturer = mId };
            //UserName 无实际意义
            user.UserName = Guid.NewGuid().ToString().Replace('-', '_');//生成guid，填充UserName
            var result = UserManager.Create(user);
            return result.Succeeded;
        }

        /// <summary>
        /// 添加角色到厂商
        /// </summary>
        /// <param name="id"></param>
        /// <returns>操作成功返回true,操作失败返回false</returns>
        public bool AddRoleToManufactruerById(string id)
        {
            var result = UserManager.AddToRole(id, "Manufactruer");
            return result.Succeeded;
        }

        /// <summary>
        /// 添加角色到普通用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns>操作成功返回true,操作失败返回false</returns>
        public bool AddRoleToOrdinaryUserById(string id)
        {
            var result = UserManager.AddToRole(id, "OrdinaryUser");
            return result.Succeeded;
        }

        /// <summary>
        /// 通过邮箱查找用户
        /// </summary>
        /// <param name="email">邮箱号</param>
        /// <returns>查找到用户返回User对象，未找到返回null</returns>
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
        public string FindUserIdByName(string name)
        {
            string id = null;
            id = db.Users.Where(x=>x.Name.Equals(name)).Select(x=>x.Id).FirstOrDefault();
            return id;
        }

        /// <summary>
        /// 通过用户名查找用户
        /// </summary>
        /// <param name="name">用户名</param>
        /// <returns>查找到用户返回User对象，未找到返回null</returns>
        public User FindUserByUserName(string name)
        {
            User user = null;
            user = UserManager.FindByName(name);
            return user;
        }

        /// <summary>
        /// 通过用户名查找用户
        /// </summary>
        /// <param name="name">用户名</param>
        /// <returns>查找到用户返回User对象，未找到返回null</returns>
        public string FindCustomUserByName(string name,string mid)
        {
            string id = null;
            id = db.Users.Where(x => (x.Name.Equals(name) && x.Manufacturer.Equals(mid))).Select(x=>x.Id).FirstOrDefault();
            return id;     
        }

        /// <summary>
        /// 通过id查找用户
        /// </summary>
        /// <param name="id">用户名</param>
        /// <returns>查找到用户返回User对象，未找到返回null</returns>
        public User FindUserById(string id)
        {
            User user = null;
            user = UserManager.FindById(id);
            return user;
        }

        /// <summary>
        /// 验证账号，通过邮箱和密码
        /// </summary>
        /// <param name="email">邮箱号</param>
        /// <param name="password">密码</param>
        /// <returns>验证成功返回User对象，验证失败返回null</returns>
        public User VerifyAccountByEmailAndPassword(string email,string password)
        {
            User user = FindUserByEmail(email);
            if (user == null)
                return user;
            user = UserManager.Find(user.UserName,password);
            return user;
        }

        /// <summary>
        /// 生成用于用户的token
        /// </summary>
        /// <param name="id"></param>
        /// <returns>正确生成token返回token,否则返回null</returns>
        public string GenerateUserToken(string id)
        {
            return UserManager.GenerateUserToken("TwoFA KEY", id);
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UpdateUser(User user)
        {
            //db.Users.Attach(user);
            //return true;
            var res = UserManager.Update(user);
            return res.Succeeded;
        }

        public int SaveToDatabase()
        {
            return db.SaveChanges();
            //db.Dispose();
        }

        /// <summary>
        /// 获取Token,登录时生成，登录成功控制器验证
        /// </summary>
        /// <param name="id">用户id</param>
        /// <returns>返回token</returns>
        public string GetTokenById(string id)
        {
            return UserManager.GenerateEmailConfirmationToken(id);
        }

        /// <summary>
        /// 解码用户名
        /// </summary>
        /// <param name="user">User对象</param>
        /// <returns>返回用户名</returns>
        public string DecodeUserName(User user)
        {
            return user.UserName.Split('_')[0];
        }

        /// <summary>
        /// 编码用户名
        /// </summary>
        /// <param name="appId">厂商id</param>
        /// <param name="name">用户名</param>
        /// <returns>返回用户名</returns>
        public string EncodeUserName(string appId,string name)
        {
            //厂商Id
            string appID = appId.Replace('-', '_');
            //用户名
            string userName = name + "_" + appID;
            return userName;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="user">User对象</param>
        public void UserSignIn(User user)
        {
            ClaimsIdentity ident = UserManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            AuthManager.SignIn(new AuthenticationProperties { IsPersistent = false }, ident);
            //HttpContext.Response.Cookies.Add(new HttpCookie("UserName",u.UserName));
        }

        /// <summary>
        /// 用户注销登录
        /// </summary>
        public void UserSignOut()
        {
            AuthManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            //var cookie = Request.Cookies["UserName"];
            //cookie.Expires = DateTime.Now.AddDays(-1);
            //Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// 通过id获取ReturnURL
        /// </summary>
        /// <param name="id"></param>
        /// <returns>返回ReturnURL，未找到则返回null</returns>
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
        /// 设置ReturnURL
        /// </summary>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <returns>操作成功返回true,操作失败返回false</returns>
        public bool SetReturnURLById(string id,string url)
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

        public bool SetTwoFAKeyById(string id,string mId,string twoFAKey)
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
            var addResult = UserManager.AddLogin(id, new UserLoginInfo(mId,twoFAKey));
            return addResult.Succeeded;
        }

        /// <summary>
        /// 生成重置密码token
        /// </summary>
        /// <param name="id"></param>
        /// <returns>正确生成token返回token,否则返回null</returns>
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
        public bool ResetPassword(string id,string token,string password)
        {
            var result = UserManager.ResetPassword(id,token, password);
            return result.Succeeded;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        /// <returns>操作成功返回true,操作失败返回false</returns>
        public bool ChangePassword(string id,string oldPassword,string newPassword)
        {
            var result = UserManager.ChangePassword(id,oldPassword, newPassword);
            return result.Succeeded;
        }

        /// <summary>
        /// 获取重置密码token
        /// </summary>
        /// <param name="id"></param>
        /// <returns>成功获取到token，返回token，否则返回null</returns>
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
        public bool SetPasswordResetTokenById(string id,string token)
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
            if(token != null && tokenTemp != null && token.Equals(tokenTemp))
            {
                var result = UserManager.RemoveClaim(id, claimTmp);
                return result.Succeeded;
            }
            return false;
        }
    }
}