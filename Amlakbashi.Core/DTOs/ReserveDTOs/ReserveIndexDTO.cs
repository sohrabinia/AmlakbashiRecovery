using Amlakbashi.Core.DTOs.UserDTOs;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.ReserveDTOs
{
    public class ReserveIndexDTO
    {
        public List<ReserveAdminItemDTO> ReserveList { get; set; }
        public PagingDTO PagingInfo { get; set; }
        public int Page { get; set; } = 1;
        public int PageItemCount { get; set; } = 20;
        public int RowIndexStart { get { return (Page * PageItemCount) - PageItemCount; } }
        public long ReserveId { get; set; } = 0;
        public long AdvertiseId { get; set; } = 0;
        public int HostUserId { get; set; } = 0;
        public int GuestUserId { get; set; } = 0;
        public int ReserveStatus { get; set; } = -1;
        public int HostResponseStatus { get; set; } = -1;
        public int GeneralStatus { get; set; } = -1;
        public string SiteClearingDate { get; set; } = "";
        public int SiteClearedStatus { get; set; } = -1;
        public string ReserveFromDate { get; set; } = "";
        public string ReserveToDate { get; set; } = "";
        public string ReserveEndDate { get; set; } = "";
        public int StayDurationFrom { get; set; } = 0;
        public int StayDurationTo { get; set; } = 0;
        public int ReserveSupportStatus { get; set; } = 0;
        public bool ShouldFollow { get; set; } = false;
        public int SupporterId { get; set; } = -1;
        public int HostCardStatus { get; set; } = -1;
        public int MainFilter { get; set; } = 0;
        public int InstantReserveFilter { get; set; } = 2;
        public bool DisableAutoCancel { get; set; } = false;
        public bool AccVisited { get; set; } = false;
        public List<UserFullNameDTO> SupporterList { get; set; }
    }
}
