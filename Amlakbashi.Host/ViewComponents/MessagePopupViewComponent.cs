using Amlakbashi.Core.Entities;
using Amlakbashi.Host.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amlakbashi.Host.ViewComponents
{
    public class MessagePopupViewComponent : ViewComponent
    {
        private readonly IUserAccessor userAccessor;
        public MessagePopupViewComponent(IUserAccessor userAccessor)
        {
            this.userAccessor = userAccessor;
        }

        public IViewComponentResult Invoke()
        {
            List<string> messages = new List<string>();

            var jointlyParking = CheckJointlyParking();
            if (string.IsNullOrEmpty(jointlyParking) == false)
            {
                messages.Add(jointlyParking);
            }

            return View(messages);
        }

        private string CheckJointlyParking()
        {
            var advertisesWithJoinlyParking = userAccessor.CurrentUser.Advertises.Where(w => w.Parking == Advertise.ParkingItems.Jointly &&
                w.Status != Advertise.AdvertiseStatus.Deleted);
            if (advertisesWithJoinlyParking.Count() > 0)
            {
                var message = "کاربر گرامی، با توجه به حذف گزینه پارکینگ مشاع از آگهی ها، لطفا نسبت به آپدیت گزینه پارکینگ آگهی های مقابل اقدام فرمایید:";
                foreach (var item in advertisesWithJoinlyParking)
                {
                    message = message + $" {item.Id}";
                }
                return message;
            }
            return null;
        }
    }
}
