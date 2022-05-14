using Amlakbashi.Core.Common.StaticData;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.Common.Utilities
{
    public static class TokenUtility
    {
        public static TokenValidationParameters GetTokenValidationParameters(string jwtSecret, bool validateLifeTime = true)
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = validateLifeTime,
                ValidateIssuerSigningKey = true,
                RequireSignedTokens = true,
                ValidIssuer = GeneralData.WebsiteUrl,
                ValidAudience = GeneralData.WebsiteUrl,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
                ClockSkew = TimeSpan.Zero
            };
        }
    }
}
