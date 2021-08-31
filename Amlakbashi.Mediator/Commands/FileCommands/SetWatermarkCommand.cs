using MediatR;

namespace Amlakbashi.Mediator.Commands.FileCommands
{
    public class SetWatermarkCommand : IRequest<string>
    {
        public long FileId { get; set; }
        public SetWatermarkCommand(long fileId)
        {
            FileId = fileId;
        }
    }
}
