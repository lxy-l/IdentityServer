
OAuth 2.0������authorization�Ĺ�ҵ��׼Э��. ����ע����ΪwebӦ��, ����Ӧ��, �ƶ�Ӧ�õ��ṩ�ض���authorization���̲���֤�����ļ���.

Authorization����˼����Ȩ, ����ʾ���ǿ�����Ȩ��ĳЩ�û�, �Ա������ܷ���һЩ����, Ҳ�����ṩ����Ȩ.

OAuth�ǻ���token��Э��.

��Щ�˿��ܶ�Authorization��Authentication�ֲ���, ���潲��authorization, ��authentication����֤������˭, ����ʹ���û�����������е�¼����authentication.

OAuthֻ����Authorization. ��ô˭������Authentication��?

�Ǿ���OpenId Connect, OpenId Connect�Ƕ�OAuth��һ�ֲ���, ��Ϊ���ܽ���Authentication.

OpenId Connect ��λ��OAuth 2.0�ϵ�һ���򵥵���֤��, ������ͻ���ʹ��authorization server��authentication��������֤�ն��û������, ͬʱҲ���Ի�ȱ�ն˿ͻ���һЩ������Ϣ.



OAuth 2.0 RFC 6794
RC 6749�ĵ�: https://tools.ietf.org/html/rfc6749

OAuthͨ�������¼���endpoint:

1. /authorize, ����token(ͨ���ض�������flows)

2. /token, ����token(ͨ���ض�������flows), ˢ��token, ʹ��authorization code����ȡtoken.

3. /revocation, ����token.

OpenId Connect ͨ�������¼��� endpoints:

1. /userinfo, ��ȡ�û���Ϣ

2. /checksession, ��鵱ǰ�û���session

3. /endsession, �սᵱǰ�û���session

4. /.well-known/openid-configuration, �ṩ��authorization server����Ϣ(endpoints�б��������Ϣ��)

5. /.well-known/jwks, �г���JWTǩ��key����Ϣ, ������������֤token��.