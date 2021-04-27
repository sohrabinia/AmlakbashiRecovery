using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amlakbashi.Core.Entities;

namespace Amlakbashi.Core.DTOs.UserDTOs
{
    public class UserIndexDTO
    {
        public IList<UserIndexItemDTO> UserItems { get; set; }
        public int Code { get; set; }
        public string Mobile { get; set; }
        public string Uname { get; set; }
        public int Photo { get; set; }
        public string Username { get; set; }
        public int Ownership { get; set; }
        public int SortOrder { get; set; }
        public int MobileStatus { get; set; }
        public int Status { get; set; }
        public int AdvertiseCount { get; set; }
        public int CompleteProfileStatus { get; set; }
        public int CompleteProfileContactStatus { get; set; }
        //public int AccessType { get; set; }
        public int UserGeneralType { get; set; }
        public int Province { get; set; }
        public int City { get; set; }
        public int Area { get; set; }
        public long AdvertiseId { get; set; }
        public User.UserFilterType UserFilterType { get; set; }
        public int CardStatus { get; set; }
        public string MinReserveNorouzFromDate { get; set; }
        public int RowIndexStart { get; set; }
    }

    public class UserIndexItemDTO
    {
        public User User { get; set; }
        public User.UserState State { get; set; }
        public int InstantReserveCancel { get; set; }
        public BankCard BankCard { get; set; }
    }
}
