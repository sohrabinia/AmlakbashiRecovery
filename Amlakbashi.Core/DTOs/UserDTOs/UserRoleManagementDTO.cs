using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.UserDTOs
{
    public class UserRoleManagementDTO
    {
        public int UserId { get; set; }
        public string Fullname { get; set; }
        public string MainMobile { get; set; }
        public IList<string> AllRoles { get; set; }
        public IList<string> CurrentRoles { get; set; }
    }
}
