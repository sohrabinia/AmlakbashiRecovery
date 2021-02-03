using MediatR;

namespace Amlakbashi.Mediator.Commands.AdvertiseCommands
{
    public class UpdateCategoryAccCountCommand : IRequest
    {
        public int categoryId { get; private set; }
        public UpdateCategoryAccCountCommand(int categoryId)
        {
            this.categoryId = categoryId;
        }
    }
}
