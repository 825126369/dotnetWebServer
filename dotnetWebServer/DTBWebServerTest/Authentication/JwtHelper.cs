using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DTBWebServer.Authentication
{
    public class JwtHelper
    {
        /// <summary>
        /// Converts an existing Jwt Token to a string
        /// </summary>
        public static string GetJwtTokenString(JwtSecurityToken token)
        {
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Returns an issuer key
        /// </summary>
        public static SymmetricSecurityKey GetSymetricSecurityKey(string issuerKey)
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(issuerKey));
        }

        public static SigningCredentials GetSigningCredentials(string issuerKey)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(issuerKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            return creds;
        }
    }
}
