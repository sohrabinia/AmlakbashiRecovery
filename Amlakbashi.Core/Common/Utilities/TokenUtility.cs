using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.Common.Utilities
{
    public static class TokenUtility
    {
        public static TokenValidationParameters GetTokenValidationParameters(string jwtSecret)
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                RequireSignedTokens = true,
                ValidIssuer = "https://www.amlakbashi.com",
                ValidAudience = "https://www.amlakbashi.com",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
                ClockSkew = TimeSpan.Zero
            };
        }
    }
}
