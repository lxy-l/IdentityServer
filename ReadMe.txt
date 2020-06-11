
OAuth 2.0是用于authorization的工业标准协议. 它关注的是为web应用, 桌面应用, 移动应用等提供特定的authorization流程并保证开发的简单性.

Authorization的意思是授权, 它表示我们可以授权给某些用户, 以便他们能访问一些数据, 也就是提供访问权.

OAuth是基于token的协议.

有些人可能对Authorization和Authentication分不清, 上面讲了authorization, 而authentication则是证明我是谁, 例如使用用户名和密码进行登录就是authentication.

OAuth只负责Authorization. 那么谁来负责Authentication呢?

那就是OpenId Connect, OpenId Connect是对OAuth的一种补充, 因为它能进行Authentication.

OpenId Connect 是位于OAuth 2.0上的一个简单的验证层, 它允许客户端使用authorization server的authentication操作来验证终端用户的身份, 同时也可以或缺终端客户的一些基本信息.



OAuth 2.0 RFC 6794
RC 6749文档: https://tools.ietf.org/html/rfc6749

OAuth通常有以下几种endpoint:

1. /authorize, 请求token(通过特定的流程flows)

2. /token, 请求token(通过特定的流程flows), 刷新token, 使用authorization code来换取token.

3. /revocation, 吊销token.

OpenId Connect 通常有以下几种 endpoints:

1. /userinfo, 获取用户信息

2. /checksession, 检查当前用户的session

3. /endsession, 终结当前用户的session

4. /.well-known/openid-configuration, 提供了authorization server的信息(endpoints列表和配置信息等)

5. /.well-known/jwks, 列出了JWT签名key的信息, 它们是用来验证token的.