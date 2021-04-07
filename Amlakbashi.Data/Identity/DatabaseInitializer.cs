using Amlakbashi.Core.Identity;
using Amlakbashi.Core.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using static Amlakbashi.Core.Entities.User;

namespace Amlakbashi.Data.Identity
{
    public static class DatabaseInitializer
    {
        public static void SeedData(IServiceScope serviceScope)
        {
            var identityContext = serviceScope.ServiceProvider.GetRequiredService<IdentityDB>();
            if (identityContext.Database.GetPendingMigrations().Any() == false)
            {
                return;
            }

            identityContext.Database.Migrate();
            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
            var roleManagerRoles = roleManager.Roles.Select(s => s.Name).ToList();
            if (roleManagerRoles.Count < Roles.AllEmployeeRoles.Length)
            {
                var rolesToCreate =
                    Roles.AllEmployeeRoles.Where(w => roleManagerRoles.Contains(w) == false);
                foreach (var roleName in rolesToCreate)
                {
                    var role = new AppRole()
                    {
                        Name = roleName
                    };
                    roleManager.CreateAsync(role).Wait();
                }
            }

            if (userManager.Users.Any() == false)
            {
                string connectionString = "Server=.;Database=amlakbas_db;Trusted_Connection=True;User Id=sa;Password=Omid@123;MultipleActiveResultSets=true;";
                string query = "select MainMobile, Email, CreateDate, State from Users";
                List<AppUser> appUserList = new List<AppUser>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var appUser = new AppUser()
                        {
                            UserName = reader[0].ToString(),
                            Email = reader[1].ToString(),
                            PhoneNumber = reader[0].ToString(),
                            PhoneNumberConfirmed = true,
                            State = (UserState)reader[3],
                        };

                        appUser.NormalizedUserName = appUser.UserName.ToUpper();
                        appUser.NormalizedEmail = appUser.Email.ToUpper();

                        if (!string.IsNullOrEmpty(reader[2].ToString()))
                        {
                            appUser.CreateDate = DateTime.Parse(reader[2].ToString());
                        }
                        else
                        {
                            appUser.CreateDate = DateTime.Now;
                        }

                        appUser.EmailConfirmed = string.IsNullOrEmpty(appUser.Email) &&
                            appUser.State == UserState.Acticved ? false : true;
                        appUser.PhoneNumberConfirmed = true;

                        appUserList.Add(appUser);
                    }
                    reader.DisposeAsync();
                    command.Dispose();
                }

                CreateRange(appUserList);
                foreach (var userRole in Roles.InitialUserRoles)
                {
                    var user = userManager.FindByNameAsync(userRole.Key).Result;
                    userManager.AddToRoleAsync(user, userRole.Value).Wait();
                }
            }
        }

        private static IdentityResult CreateRange(IList<AppUser> userList)
        {
            string connectionString = "Server=.;Database=Amlakbashi.Identity;Trusted_Connection=True;User Id=sa;Password=Omid@123;MultipleActiveResultSets=true;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;
                    command.CommandText = "INSERT INTO AspnetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PhoneNumber, PhoneNumberConfirmed, AccessFailedCount, TwoFactorEnabled, LockoutEnabled, CreateDate, SecurityStamp, ConcurrencyStamp, State) VALUES (@id, @username, @normalizedusername, @email, @normalizedemail, @emailconfirmed, @phonenumber, @phonenumberconfirmed, @accessfailed, @twofactorenabled, @lockoutenabled, @createdate, @securitystamp, @concurrencystamp, @state);";
                    command.Parameters.Add("@id", System.Data.SqlDbType.NVarChar);
                    command.Parameters.Add("@username", System.Data.SqlDbType.NVarChar);
                    command.Parameters.Add("@normalizedusername", System.Data.SqlDbType.NVarChar);
                    command.Parameters.Add("@email", System.Data.SqlDbType.NVarChar);
                    command.Parameters.Add("@normalizedemail", System.Data.SqlDbType.NVarChar);
                    command.Parameters.Add("@emailconfirmed", System.Data.SqlDbType.Bit);
                    command.Parameters.Add("@phonenumber", System.Data.SqlDbType.NVarChar);
                    command.Parameters.Add("@phonenumberconfirmed", System.Data.SqlDbType.Bit);
                    command.Parameters.Add("@accessfailed", System.Data.SqlDbType.Int);
                    command.Parameters.Add("@twofactorenabled", System.Data.SqlDbType.Bit);
                    command.Parameters.Add("@lockoutenabled", System.Data.SqlDbType.Bit);
                    command.Parameters.Add("@createdate", System.Data.SqlDbType.DateTime2);
                    command.Parameters.Add("@securitystamp", System.Data.SqlDbType.NVarChar);
                    command.Parameters.Add("@concurrencystamp", System.Data.SqlDbType.NVarChar);
                    command.Parameters.Add("@state", System.Data.SqlDbType.NVarChar);
                    foreach (var item in userList)
                    {
                        command.Parameters["@id"].Value = item.Id;
                        command.Parameters["@username"].Value = item.UserName;
                        command.Parameters["@normalizedusername"].Value = item.NormalizedUserName;
                        command.Parameters["@email"].Value = item.Email;
                        command.Parameters["@normalizedemail"].Value = item.NormalizedEmail;
                        command.Parameters["@emailconfirmed"].Value = item.EmailConfirmed;
                        command.Parameters["@phonenumber"].Value = item.PhoneNumber;
                        command.Parameters["@phonenumberconfirmed"].Value = item.PhoneNumberConfirmed;
                        command.Parameters["@accessfailed"].Value = item.AccessFailedCount;
                        command.Parameters["@twofactorenabled"].Value = item.TwoFactorEnabled;
                        command.Parameters["@lockoutenabled"].Value = item.LockoutEnabled;
                        command.Parameters["@createdate"].Value = item.CreateDate;
                        command.Parameters["@securitystamp"].Value = item.SecurityStamp;
                        command.Parameters["@concurrencystamp"].Value = item.ConcurrencyStamp;
                        command.Parameters["@state"].Value = (int)item.State;
                        command.ExecuteNonQuery();
                    }
                }
            }
            return IdentityResult.Success;
        }
    }
}
