using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs;
using Amlakbashi.Core.Entities;
using MediatR;

namespace Amlakbashi.Mediator.Events.AdvertiseEvents
{
    public class ChangeAdvertiseAddressEvent : INotification
    {
        public long advertiseId { get; set; }
        public bool IsAdvertiseActive { get; set; }
        public AddressInputDTO prevAddress { get; set; }
        public AddressInputDTO newAddress { get; set; }
        
        public ChangeAdvertiseAddressEvent(Advertise prevAdvertise, Advertise newAdvertise)
        {
            IsAdvertiseActive = prevAdvertise.IsActive;
            advertiseId = newAdvertise.Id;
            PropertyCopier<Advertise, AddressInputDTO>.Copy(prevAdvertise, prevAddress);
            PropertyCopier<Advertise, AddressInputDTO>.Copy(newAdvertise, newAddress);
        }
    }
}
