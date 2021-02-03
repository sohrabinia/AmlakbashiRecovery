using MediatR;

namespace Amlakbashi.Mediator.Commands.AdvertiseCommands
{
    public class UpdateUserScoreCommand : IRequest
    {
        public int UserId { get; set; }
        public UpdateUserScoreCommand(int userId)
        {
            this.UserId = userId;
        }
    }
}
