using Amlakbashi.Core.Identity.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Amlakbashi.Data.Identity
{
    public class IdentityDB : IdentityDbContext<AppUser, AppRole, string>
    {
        public IdentityDB(DbContextOptions<IdentityDB> options) : base(options)
        {

        }
    }
}
