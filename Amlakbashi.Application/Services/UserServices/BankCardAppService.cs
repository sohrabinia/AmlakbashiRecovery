using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using static Amlakbashi.Core.Entities.BankCard;
using Amlakbashi.Mediator.Events.UserEvents;

namespace Amlakbashi.Application.Services.UserServices
{
    internal class BankCardAppService : BaseAppService<BankCard, int>, IBankCardAppService
    {
        private readonly IMediator mediator;
        public BankCardAppService(IRepository<BankCard, int> repository, IMediator mediator) : base(repository)
        {
            this.mediator = mediator;
        }

        public IQueryable<BankCard> GetAll()
        {
            return Repository.Query(q => q);
        }

        public IList<BankCard> Filter(int user_id, string bank_card_number,
            string shaba_number, int bank_card_status, int shaba_status)
        {
            IQueryable<BankCard> model = Repository.Query(q => q);
            if (user_id > 0)
            {
                model = model.Where(x => x.UserID == user_id);
            }
            if (!string.IsNullOrEmpty(bank_card_number))
            {
                model = model.Where(x => x.BankCardNumber == bank_card_number);
            }
            if (!string.IsNullOrEmpty(shaba_number))
            {
                model = model.Where(x => x.ShabaNumber == shaba_number);
            }
            if (bank_card_status > -1)
            {
                model = model.Where(x => x.BankCardStatus == bank_card_status);
            }
            if (shaba_status > 0)
            {
                model = model.Where(x => x.ShabaStatus == shaba_status);
            }
            model = model.OrderByDescending(x => x.Id);
            return model.ToList();
        }

        public BankCard Find(int id)
        {
            return Repository.Query(q => q.FirstOrDefault(x => x.Id == id));
        }

        public BankCard GetByUserId(int userId)
        {
            return Repository.Query(q => q.FirstOrDefault(x => x.UserID == userId));
        }

        public void Insert(BankCard bankCard, int currentUserId, ActionLog.ActionSourceEnum source)
        {
            Repository.Insert(bankCard);
            Repository.Save();
            mediator.Publish(new BankCardUpdateEvent(null, bankCard, source, currentUserId));
        }

        public BankCardStatusEnum ToggleBankCardStatus(int id)
        {
            var data = Repository.Query(q => q.FirstOrDefault(x => x.Id == id));
            data.ToggleBankCardStatus();
            Repository.Save();
            return data.GetBankCardStatus();
        }

        public BankCardStatusEnum ToggleShabaStatus(int id)
        {
            var data = Repository.Query(q => q.FirstOrDefault(x => x.Id == id));
            data.ToggleShabaStatus();
            Repository.Save();
            return data.GetShabaStatus();
        }

        public void Update(BankCard editedBankCard, int currentUserId, ActionLog.ActionSourceEnum source)
        {
            var bankCard = Repository.Query(q => q.FirstOrDefault(f => f.Id == editedBankCard.Id));
            var shallowBankCard = bankCard.ShallowCopy();
            bankCard.BankCardNumber = editedBankCard.BankCardNumber;
            bankCard.FName = editedBankCard.FName;
            bankCard.LName = editedBankCard.LName;
            bankCard.LastModifyDate = DateTime.Now;
            Repository.Update(bankCard);
            Repository.Save();
            mediator.Publish(new BankCardUpdateEvent(shallowBankCard, bankCard, source, currentUserId));
        }
    }
}
