using System.Collections.Generic;
using System.Linq;
using Amlakbashi.Core.Entities;

namespace Amlakbashi.Application.Services.UserServices.Interfaces
{
    public interface IBankCardAppService
    {
        IQueryable<BankCard> GetAll();
        IList<BankCard> Filter(int user_id, string bank_card_number, string shaba_number,
            int bank_card_status, int shaba_status);
        BankCard Find(int id);
        BankCard GetByUserId(int userId);
        void Insert(BankCard bankCard, int currentUserId, ActionLog.ActionSourceEnum source);
        BankCard.BankCardStatusEnum ToggleBankCardStatus(int id);
        BankCard.BankCardStatusEnum ToggleShabaStatus(int id);
        void Update(BankCard editedBankCard, int currentUserId, ActionLog.ActionSourceEnum source);
    }
}
