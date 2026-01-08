using DTBWebServer.Authentication;
using DTBWebServer.GameLogic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DTBWebServer.Controllers.Authenticate
{
    [Route($"{GameConst.routePrefix}/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly JwtConfig jwtconfig;
        public AuthenticateController(IOptions<JwtConfig> option)
        {
            jwtconfig = option.Value;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Authenticate")]
        public async Task<JsonResult> Authenticate()
        {
            AuthenticationResultData mResult = new AuthenticationResultData();
            try
            {
                IFormCollection collection = await Request.ReadFormAsync();
                if (!collection.ContainsKey("uId") || !collection.ContainsKey("pwd"))
                {
                    mResult.nErrorCode = NetErrorCode.ParamError;
                    return new JsonResult(mResult);
                }

                string userId = collection["uId"];
                string password = collection["pwd"];
                if (string.IsNullOrWhiteSpace(userId) || userId.Length < 6)
                {
                    mResult.nErrorCode = NetErrorCode.ParamError;
                    return new JsonResult(mResult);
                }

                if (password != jwtconfig.pwd)
                {
                    mResult.nErrorCode = NetErrorCode.ParamError;
                    return new JsonResult(mResult);
                }

                var claims = new List<Claim>();
                claims.Add(new Claim("userId", userId));
                claims.Add(new Claim("password", password));

                JwtSecurityToken token = new JwtSecurityToken(
                    issuer: jwtconfig.Issuer,
                    audience: jwtconfig.Audience,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromHours(5)),
                    claims: claims,
                    signingCredentials: JwtHelper.GetSigningCredentials(jwtconfig.SigningKey)
                );

                mResult.nErrorCode = NetErrorCode.Success;
                mResult.token = JwtHelper.GetJwtTokenString(token);
                mResult.expires = TimeUtility.GetTimeStampFromUTCTime(token.ValidTo);
                return new JsonResult(mResult);
            }
            catch (Exception ex)
            {
                mResult.nErrorCode = NetErrorCode.ServerInnerEexception;
                mResult.errorMsg = ex.Message;
                return new JsonResult(mResult);
            }
        }

    }
}
