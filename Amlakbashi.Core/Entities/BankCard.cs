using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Amlakbashi.Core.Common.Entity;
using Newtonsoft.Json;

namespace Amlakbashi.Core.Entities
{
    public class BankCard : Entity<int>, ISoftDelete
    {
        [Column("BankCardID")]
        public override int Id { get; set; }
        public int UserID { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string BankCardNumber { get; set; }
        public string ShabaNumber { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastModifyDate { get; set; }
        public int BankCardStatus { get; set; }
        public int ShabaStatus { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("UserID")]
        [JsonIgnore]
        public virtual User User { get; set; }

        [NotMapped]
        public string FullName 
        {
            get
            {
                return (!string.IsNullOrEmpty(FName) ? FName + " " : "") +
                    (!string.IsNullOrEmpty(LName) ? LName : "");
            }
        }

        public BankCard ShallowCopy()
        {
            return (BankCard)this.MemberwiseClone();
        }

        public BankCardStatusEnum GetBankCardStatus()
        {
            return (BankCardStatusEnum)BankCardStatus;
        }

        public void SetBankCardStatus(BankCardStatusEnum value) 
        { 
            BankCardStatus = (int)value;
        }

        public void ToggleBankCardStatus()
        {
            SetBankCardStatus(GetBankCardStatus() == BankCardStatusEnum.Verified ?
                BankCardStatusEnum.NotVerified : BankCardStatusEnum.Verified);
        }

        public BankCardStatusEnum GetShabaStatus()
        {
            return (BankCardStatusEnum)ShabaStatus;
        }

        public void SetShabaStatus(BankCardStatusEnum value)
        {
            ShabaStatus = (int)value;
        }

        public void ToggleShabaStatus()
        {
            SetShabaStatus(GetShabaStatus() == BankCardStatusEnum.Verified ?
                BankCardStatusEnum.NotVerified : BankCardStatusEnum.Verified);
        }

        public enum BankCardStatusEnum
        {
            Verified = 0,
            NotVerified = 1
        }
    }
}
