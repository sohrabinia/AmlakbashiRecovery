using Amlakbashi.Core.Common.ContactEngines;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.UserContact.Interfaces;

namespace Amlakbashi.Core.Infrastructure.UserContact
{
    public class SmsContact : ISmsContact
    {
        public void SendMessage(UserContactDTO contactDTO)
        {
            var number_is_for_iran = PhoneUtility.IsNumberForIran(contactDTO.UserMainMobile);
            var mobile_to_send_sms = number_is_for_iran ?
                PhoneUtility.InternationalNumberToLocal(contactDTO.UserMainMobile) :
                PhoneUtility.InternationalNumberToCallable(contactDTO.UserMainMobile);
            switch (contactDTO.Type)
            {
                case UserContactType.confirm:
                    SmsEngine.VerifyLookup(mobile_to_send_sms,
                        contactDTO.AdvertiseId, contactDTO.Type.ToString());
                    break;
                case UserContactType.RefuseCancelReserve:
                case UserContactType.HostReserveCanceled:
                case UserContactType.HostCancelRequestSent:
                case UserContactType.GuestStayStarted:
                case UserContactType.GuestReserveRejected:
                case UserContactType.GuestReserveCanceledByHost:
                case UserContactType.GuestReserveCanceled:
                case UserContactType.GuestRefuseCancelReserveByHost:
                case UserContactType.GuestCancelRequestSent:
                    SmsEngine.VerifyLookup(mobile_to_send_sms,
                        contactDTO.AdvertiseId, contactDTO.ReserveId, contactDTO.Type.ToString());
                    break;
                case UserContactType.NewReserveChatGuest:
                case UserContactType.NewReserveChatHost:
                    break;
                case UserContactType.GuestPayReserve:
                case UserContactType.HostReserveCashPay:
                    SmsEngine.VerifyLookup(mobile_to_send_sms,
                        contactDTO.AdvertiseId, contactDTO.UserId, contactDTO.ReserveId, contactDTO.Type.ToString());
                    break;
                case UserContactType.GuestReservedTotalPayed:
                case UserContactType.HostReservedTotalPayed:
                    SmsEngine.VerifyLookup(mobile_to_send_sms,
                        contactDTO.AdvertiseId, contactDTO.AudienceMobile, contactDTO.ReserveId, contactDTO.Type.ToString());
                    break;
                case UserContactType.GuestReservedDepositePayed:
                    SmsEngine.VerifyLookup(mobile_to_send_sms,
                        contactDTO.AdvertiseId, contactDTO.Price, contactDTO.RemainPrice,
                        contactDTO.AudienceMobile, contactDTO.ReserveId, contactDTO.Type.ToString());
                    break;
                case UserContactType.HostReservedDepositePayed:
                    SmsEngine.VerifyLookup(mobile_to_send_sms,
                        contactDTO.AdvertiseId, contactDTO.RemainPrice, contactDTO.AudienceMobile,
                        contactDTO.Price, contactDTO.ReserveId, contactDTO.Type.ToString());
                    break;
                case UserContactType.ReserveCanceledBySystem:
                    SmsEngine.VerifyLookup(mobile_to_send_sms,
                        contactDTO.AdvertiseId, contactDTO.DoerTitle, contactDTO.ReserveId, contactDTO.Type.ToString());
                    break;
                case UserContactType.ReserveRequest:
                    SmsEngine.VerifyLookup(mobile_to_send_sms,
                                contactDTO.UserId, contactDTO.AdvertiseId, contactDTO.Extra1, contactDTO.Extra2,
                                contactDTO.Extra3, contactDTO.Type.ToString());
                    break;
                case UserContactType.SiteClearingHost:
                case UserContactType.SiteClearingHostWithCredit:
                    SmsEngine.VerifyLookup(mobile_to_send_sms,
                        contactDTO.Price, contactDTO.AdvertiseId, contactDTO.TransactionId, contactDTO.ReserveId, contactDTO.Type.ToString());
                    break;
                case UserContactType.SiteRefundGuest:
                    SmsEngine.VerifyLookup(mobile_to_send_sms,
                        contactDTO.Price, contactDTO.AdvertiseId, contactDTO.TransactionId, contactDTO.ReserveId, contactDTO.Type.ToString());
                    break;
                case UserContactType.HostReserveRejectedForReserved:
                    SmsEngine.VerifyLookup(mobile_to_send_sms,
                        contactDTO.AdvertiseId, contactDTO.ReserveId, contactDTO.Type.ToString());
                    break;
                case UserContactType.payment:
                    SmsEngine.VerifyLookup(mobile_to_send_sms,
                        contactDTO.TransactionId, contactDTO.Type.ToString());
                    break;
                case UserContactType.UserCreditIncrease:
                case UserContactType.UserCreditDecrease:
                    SmsEngine.VerifyLookup(mobile_to_send_sms,
                        contactDTO.Price, "", contactDTO.TransactionId, contactDTO.CauseString, contactDTO.Type.ToString());
                    break;
                case UserContactType.FinishStay:
                    SmsEngine.VerifyLookup(mobile_to_send_sms,
                        contactDTO.Extra1, "", "", contactDTO.Extra2, contactDTO.Type.ToString());
                    break;
                case UserContactType.PrizeCharge:
                case UserContactType.CouponAppreciate:
                    SmsEngine.VerifyLookup(mobile_to_send_sms,
                        contactDTO.Extra1, contactDTO.Type.ToString());
                    break;
                case UserContactType.CouponPresent:
                    SmsEngine.VerifyLookup(mobile_to_send_sms,
                        contactDTO.Extra2, "", "", contactDTO.Extra1, contactDTO.Type.ToString());
                    break;
                case UserContactType.HostUpdatePrice:
                    SmsEngine.VerifyLookup(mobile_to_send_sms,
                        ".", "", "", contactDTO.Extra1, "SetNorouzPrice");
                    break;
                case UserContactType.GuestNorouzRules:
                    SmsEngine.VerifyLookup(mobile_to_send_sms,
                        ".", "", "", contactDTO.Extra1, "GuestNorouzRules");
                    break;
            }
        }

        public void SendReserveRequestCall(User user, long advertiseId)
        {
            SmsEngine.VerifyLookup(
                PhoneUtility.IsNumberForIran(user.GetPhoneNumber(User.PhoneType.MainMobile)) ?
                user.GetLocalPhoneNumber(User.PhoneType.MainMobile) : user.GetCallablePhoneNumber(User.PhoneType.MainMobile),
                advertiseId.ToString(), "ReserveRequestCall", Kavenegar.Core.Models.Enums.VerifyLookupType.Call);
        }

        public void SendPayReserveCall(User user, long advertiseId)
        {
            SmsEngine.VerifyLookup(
                PhoneUtility.IsNumberForIran(user.GetPhoneNumber(User.PhoneType.MainMobile)) ?
                user.GetLocalPhoneNumber(User.PhoneType.MainMobile) : user.GetCallablePhoneNumber(User.PhoneType.MainMobile),
                advertiseId.ToString(), "GuestPayReserveCall", Kavenegar.Core.Models.Enums.VerifyLookupType.Call);
        }

        public void SendTemplate(string mobile, string template)
        {
            SmsEngine.SendSms(mobile, template);
        }

        public void SendVerification(string localNumber, string code)
        {
            SmsEngine.SendVerification(localNumber, code);
        }
    }
}
