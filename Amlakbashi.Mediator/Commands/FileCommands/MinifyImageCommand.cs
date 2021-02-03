using MediatR;

namespace Amlakbashi.Mediator.Commands.FileCommands
{
    public class MinifyImageCommand : IRequest
    {
        public string ImagePath { get; set; }
        public int MaxWidth { get; set; }
        public long QualityPercent { get; set; }
        public long FileId { get; set; }
        public MinifyImageCommand(string imagePath, int maxWidth, long qualityPercent, long fileId)
        {
            this.ImagePath = imagePath;
            this.MaxWidth = maxWidth;
            this.QualityPercent = qualityPercent;
            this.FileId = fileId;
        }
    }
}
