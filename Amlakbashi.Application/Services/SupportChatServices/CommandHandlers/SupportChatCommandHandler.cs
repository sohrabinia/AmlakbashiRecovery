using Amlakbashi.Core.Common.Extensions;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using Amlakbashi.Mediator.Commands.SupportChatCommands;
using Amlakbashi.Mediator.Commands.UserCommands;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Amlakbashi.Application.Services.SupportChatServices.CommandHandlers
{
    public class SupportChatCommandHandler : IRequestHandler<SendSupporterMessageCommand>
    {
        //public static int[] admin_ids = new int[] { 3, 12, 1667, 8851, 2122, 39297, 42640, 40915, 36657,
        //19076, 17244, 48018, 78931 };
        public static int[] admin_ids = new int[] { 3, 12, 1667, 8851, 2122, 19076, 17244, 36657, 42640,
            58503, 68453, 68327, 71271, 71082, 6931, 76022, 78931 };

        private readonly IMediator mediator;
        private readonly IRepository<SupportChat, long> Repository;
        public SupportChatCommandHandler(IRepository<SupportChat, long> repository,
            IMediator mediator)
        {
            Repository = repository;
            this.mediator = mediator;
        }

        public Task<Unit> Handle(SendSupporterMessageCommand request, CancellationToken cancellationToken)
        {
            var supportChat = Repository.Find(request.SupportChatId);
            var message = Repository.Find<SupportChatMessage, long>(request.MessageId);
            if (message.ReadStatus == SupportChatMessage.ReadStatusEnum.NotRead)
            {
                var text = message.Text;
                if (!string.IsNullOrEmpty(text) && text.Length > 15)
                {
                    text = text.Substring(0, 15);
                }

                string title;
                if (supportChat.UserID > 0)
                {
                    var user = Repository.Find<User, int>((int)supportChat.UserID);
                    title = user.FullName;
                    if (string.IsNullOrEmpty(title))
                    {
                        title = user.Id.ToString();
                    }
                }
                else
                {
                    title = "ناشناس";
                }

                title += " :";
                var tokens = new List<string>();
                var twoHours = new TimeSpan(0, 30, 0);
                var now = DateTime.Now;
                if (supportChat.SupporterID != null && now - supportChat.LastMessageTime < twoHours)
                {
                    var supporter = Repository.Find<User, int>((int)supportChat.SupporterID);
                    if (supporter != null && !string.IsNullOrEmpty(supporter.NotificationToken))
                    {
                        tokens.Add(supporter.NotificationToken);
                    }
                }

                if (tokens.Count == 0)
                {
                    var ids = admin_ids.ToList();
                    ids.Remove(1667);
                    ids.Remove(3);
                    ids.Remove(19076);
                    ids.Remove(17244);
                    foreach (var item in ids)
                    {
                        var user = Repository.Find<User, int>(item);
                        tokens.Add(user.NotificationToken);
                    }
                }

                for (int i = 0; i < tokens.Count; i++)
                {
                    var token = tokens[i];
                    var delay = new TimeSpan(0, 0, (i + 1));
                    mediator.Schedule(new ScheduleSendNotificationCommand(token, title, text,
                        "/supportchat/index?id=" + supportChat.Id), delay);
                }
            }
            return Task.FromResult(Unit.Value);
        }
    }
}
