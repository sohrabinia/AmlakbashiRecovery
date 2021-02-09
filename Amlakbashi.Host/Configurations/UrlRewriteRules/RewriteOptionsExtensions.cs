using Microsoft.AspNetCore.Rewrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Configurations.UrlRewriteRules
{
    public static class RewriteOptionsExtensions
    {
        public static void AddRedirectToLowerCase(this RewriteOptions rewriteOptions)
        {
            rewriteOptions.Rules.Add(new RedirectLowerCaseRule());
        }
    }
}
