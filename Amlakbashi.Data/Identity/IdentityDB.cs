using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Data.Identity
{
    public class IdentityDB : IdentityDbContext<AppUser, AppRole, string>
    {
        public IdentityDB(DbContextOptions<IdentityDB> options) : base(options)
        {

        }
    }
}
