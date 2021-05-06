using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Amlakbashi.Core.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace Amlakbashi.Host.Authentication
{
    public class CustomPasswordValidator : IPasswordValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string password)
        {
            var errors = new List<IdentityError>();
            if (Regex.IsMatch(password, "[^\u0000-\u0080]+"))
            {
                errors.Add(new IdentityError()
                {
                    Code = "PersianCharacterForbidden",
                    Description = "Password must not contain persian characters"
                });
            }
            if (Regex.IsMatch(password, "[a-zA-Z]") == false)
            {
                errors.Add(new IdentityError()
                {
                    Code = "PasswordRequiresLetter",
                    Description = "Password must contain at least one alphabetic character"
                });
            }
            if (Regex.IsMatch(password, "[0-9]+") == false)
            {
                errors.Add(new IdentityError()
                {
                    Code = "PasswordRequiresDigit",
                    Description = "Password must contain at least one numeric character"
                });
            }
            if (password.Length < 5)
            {
                errors.Add(new IdentityError()
                {
                    Code = "PasswordTooShort",
                    Description = "Password must be at least 5 characters long"
                });
            }
            if (errors.Any())
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }
            return Task.FromResult(IdentityResult.Success);
        }
    }
}