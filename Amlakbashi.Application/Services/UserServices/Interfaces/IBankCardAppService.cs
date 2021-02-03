using Amlakbashi.Core.Common.AppService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amlakbashi.Core.DTOs.UserDTOs;
using Amlakbashi.Core.Entities;
using static Amlakbashi.Core.Entities.BankCard;

namespace Amlakbashi.Application.Services.UserServices.Interfaces
{
    public interface IBankCardAppService : IAppService<BankCard,int>
    {
        IQueryable<BankCard> GetAll();
        IList<BankCard> Filter(int user_id, string bank_card_number,string shaba_number,
            int bank_card_status, int shaba_status);
        BankCard Find(int id);
        BankCard GetByUserId(int userId);
        void Insert(BankCard bankCard, int currentUserId, ActionLog.ActionSourceEnum source);
        BankCardStatusEnum ToggleBankCardStatus(int id);
        BankCardStatusEnum ToggleShabaStatus(int id);
        void Update(BankCard editedBankCard, int currentUserId, ActionLog.ActionSourceEnum source);
        void UpdateDirectly(BankCard bankCard, int currentUserId, ActionLog.ActionSourceEnum source);
    }
}
