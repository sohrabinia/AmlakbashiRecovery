using MediatR;

namespace Amlakbashi.Mediator.Commands.AdvertiseCommands
{
    public class UpdateCategoryMostAccCommand : IRequest
    {
        public int categoryId { get; private set; }
        public UpdateCategoryMostAccCommand(int categoryId)
        {
            this.categoryId = categoryId;
        }
    }
}
