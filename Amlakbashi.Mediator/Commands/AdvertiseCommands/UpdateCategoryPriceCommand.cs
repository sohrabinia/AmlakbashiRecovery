using MediatR;

namespace Amlakbashi.Mediator.Commands.AdvertiseCommands
{
    public class UpdateCategoryPriceCommand : IRequest
    {
        public int categoryId { get; private set; }
        public UpdateCategoryPriceCommand(int categoryId)
        {
            this.categoryId = categoryId;
        }
    }
}
