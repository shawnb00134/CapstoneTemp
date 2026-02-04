using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CAMCMSServer.Model;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Twilio.Jwt.AccessToken;

namespace CAMCMSServer.Utils
{
    public class TokenValidator
    {
        public static IConfiguration? Configuration;

        public static Task<bool> ValidateAccessToken(string accessToken, string userName)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String("GawgguFyGrWKav7AX4VKUg9uRc5v4p4s9D3IqEaf5x0=")),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(accessToken, validationParameters, out _);
                
                var userNamesMatch = principal.Claims.FirstOrDefault(c => c.Type == "username")?.Value == userName;

                return Task.FromResult(principal.Identity is { IsAuthenticated: true } && userNamesMatch);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error validating token");
                return Task.FromResult(false);
            }
        }

        public static string? GenerateUserAccessToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim("id", user.Id.ToString()),
                        new Claim("username", user.Username),
                    }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(Convert.FromBase64String("GawgguFyGrWKav7AX4VKUg9uRc5v4p4s9D3IqEaf5x0=")),
                        SecurityAlgorithms.HmacSha256Signature)
                };
                

                var tokenString = tokenHandler.CreateEncodedJwt(tokenDescriptor);
                return tokenString;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error generating token");
                return null;
            }

        }

        [ObsoleteAttribute]
        private static async Task<TokenValidationParameters> buildTokenValidationParameters()
        {
            var httpClient = new HttpClient();
            var keySetRequest = await httpClient.GetStringAsync(
                $"{TokenValidator.Configuration!.GetSection("AWS").GetValue<string>("CognitoKeys")}");

            var keySet = new JsonWebKeySet(keySetRequest);


            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = TokenValidator.Configuration!.GetSection("AWS").GetValue<string>("CognitoIssuer"),
                ValidateLifetime = true,
                RequireExpirationTime = true,
                ValidateAudience = false,
                ValidateActor = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = keySet.GetSigningKeys(),
                RequireSignedTokens = true
            };
            return validationParameters;
        }
    }
}
