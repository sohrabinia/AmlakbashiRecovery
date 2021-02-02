
using System;

namespace Amlakbashi.Core.Entities.Interfaces
{
    public interface IReserve
    {
        long Id { get; set; }
        long AdvertiseID { get; set; }
        Advertise Advertise { get; set; }
        int HostUserID { get; set; }
        DateTime StartDate { get; set; }
        DateTime CreateDate { get; set; }
    }
}
