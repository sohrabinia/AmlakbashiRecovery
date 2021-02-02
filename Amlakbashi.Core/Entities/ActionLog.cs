using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Entities
{
    public class ActionLog : Entity<long>
    {
        [Column("ActionLogID")]
        public override long Id { get; set; }
        public int UserID { get; set; }
        public DateTime Date { get; set; }
        public int Type { get; set; }
        public long RelatedID { get; set; }
        public int ActionSource { get; set; }
        public string PreviousData { get; set; }
        public string CurrentData { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        public enum ActionSourceEnum
        {
            Undefined = -1,
            Background = 0,
            Website = 1,
            WebsiteDashboard = 2,
            Application = 3,
            AdminPanel = 4
        }

        public enum ActionTypeEnum
        {
            Unset = -1,
            BankCard = 1,
            User = 2,
            Advertise = 3
        }

        public static string GetActionSourceString(int action_source)
        {
            return GetActionSourceString((ActionSourceEnum)action_source);
        }

        public static string GetActionSourceString(ActionSourceEnum action_source)
        {
            switch (action_source)
            {
                case ActionSourceEnum.Undefined:
                    return "همه";
                case ActionSourceEnum.Background:
                    return "بکگراند";
                case ActionSourceEnum.Website:
                    return "سایت";
                case ActionSourceEnum.WebsiteDashboard:
                    return "داشبورد سایت";
                case ActionSourceEnum.Application:
                    return "اپلیکیشن";
                case ActionSourceEnum.AdminPanel:
                    return "پنل ادمین";
                default:
                    return "";
            }
        }

        public static string GetActionTypeString(int action_type)
        {
            return GetActionTypeString((ActionTypeEnum)action_type);
        }

        public static string GetActionTypeString(ActionTypeEnum action_type)
        {
            switch (action_type)
            {
                case ActionTypeEnum.Unset:
                    return "همه";
                case ActionTypeEnum.BankCard:
                    return "حساب بانکی";
                case ActionTypeEnum.User:
                    return "کاربر";
                default:
                    return "";
            }
        }

        public static string GetActionTypeRelatedIDString(int action_type)
        {
            switch ((ActionTypeEnum)action_type)
            {
                case ActionTypeEnum.BankCard:
                case ActionTypeEnum.User:
                    return "کد کاربری";
                default:
                    return "";
            }
        }
    }
}
