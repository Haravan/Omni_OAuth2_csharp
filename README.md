Login có bình thường:
- Run project HaravanAuthorization sẽ auto redirect url /account/login đễ login.
- Hàm login_callback nhận info user sau khi login.

Login có grant_service:
- Hàm request_grant là hàm login có grant_service.
- Hàm request_grant_callback nhận info user sau khi login có grant_service.