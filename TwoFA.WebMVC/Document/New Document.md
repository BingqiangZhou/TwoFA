***第一步：[注册](https://bingqiangzhou.cn/Register/Step1)与[登录](https://bingqiangzhou.cn/Login/Login)***
- 
###1.1. 填写相关信息
根据要求填写您的邮箱，企业名称，密码（两次输入确认密码），页面如图一所示。

<div align=center>
<img src="./picture01.png"  alt="填写相关信息页面" />
</div>
<div align=center>图一、填写相关信息页面</div>
###1.2. 验证邮箱
当您完成第一步时，我们会将验证码输入到您的邮箱，您可以邮箱查看验证码并输入如下图二所示页面完成验证。
<div align=center>
<img src="./picture02.png"  alt="验证邮箱" />
</div>
<div align=center>图二、验证邮箱</div>
###1.3. 完成注册
当您看到一下页面（如图三所示）时，代表您已经注册成功了。
<div align=center>
<img src="./picture03.png"  alt="完成注册" />
</div>
<div align=center>图三、完成注册</div>
###1.4. 登录账号
当您完成注册之后，您可以输入您的邮箱以及密码进行登录（如图四所示）。
<div align=center>
<img src="./picture04.png"  alt="登录账号" />
</div>
<div align=center>图四、登录账号</div>
当您在首页看到您企业的名称之后，这时说明您已经登录成功了。（如图所示）
<div align=center>
<img src="./picture05.png"  alt="登录成功" />
</div>
<div align=center>图五、登录成功</div>
**恭喜您完成注册并登录，接下来我们进行第二步--配置。**
***第二步：[配置](http://bingqiangzhou.cn/Config)***
- 
###2.1. 配置回置链接
我们紧接着完成第二步，配置回置链接，这里的回置链接是您后台服务器对应验证成功后登录的控制器。
配置如：http://bingqiangzhou.cn/Login/LoginSuccess（**注意最后这里没有斜杠**）
<div align=center>
<img src="./picture06.png"  alt="设置回置链接" />
</div>
<div align=center>图六、设置回置链接</div>
**当出现链接已更新的时候，说明配置成功。**
***第三步：调用***
- 
**最后一步，也是最重要的一步---调用页面。**其中这一步又分为为用户开启服务以及开启服务之后登录时进行的验证服务。

###3.1. 获取Token
我们可以在配置页面获取到您的openId和token,获取到这些信息之后我们可以进行下一步。
<div align=center>
<img src="./picture07.png"  alt="设置回置链接" />
</div>
<div align=center>图七、获取token</div>
###3.2. 为用户开启服务
我们获取到相关信息之后，我们只需要访问https://bingqiangzhou.cn/OpenTwoFAService,并传入用户的唯一标识、您的openId以及token，您就可以为用户打开两步验证服务。（建议您在您的网页设置开关，随后引入到该页面）。
用户可以使用小程序WeDevelopTogether来记录这些信息，并生成秘钥，用于下次登录时的验证。
当您传入的参数正确时，您将看到下面的页面（如图八所示）。
<div align=center>
<img src="./picture08.png"  alt="开启服务页面" />
</div>
<div align=center>图八、开启服务页面</div>
###3.3. 为用户开启服务
当您的用户进行登录操作时，您需要传入用户的唯一标识、您的openId以及token以及您的accessToken(可以省略)转到https://bingqiangzhou.cn/TwoFAValidationService,如果您的用户开启了两步验证服务，我们将要求您的用户进行两步验证，如果您的用户没有开启两步验证服务，我们将不会去要求他进行验证操作，而是直接将accessToken返回到之前您配置的回置链接，您可以验证accessToken来辨别登录用户和登录的安全性，如果您不设置accesstoken,您可以设置https://bingqiangzhou.cn为唯一可访问的服务器。
当您传入的参数正确时，您将看到下面的页面（如图九所示），用户通过输入小程序生成的验证码，而完成登录。
<div align=center>
<img src="./picture09.png"  alt="两步验证服务页面" />
</div>
<div align=center>图九、两步验证服务页面</div>
###3.4. 其他服务
其他关闭两步验证服务以及重置服务等，请您具体查看[API说明](https://bingqiangzhou.cn/Graduation/Help)。
***感谢您的使用！***

