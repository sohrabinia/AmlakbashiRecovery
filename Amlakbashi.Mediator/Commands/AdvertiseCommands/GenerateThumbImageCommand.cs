using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Mediator.Commands.AdvertiseCommands
{
    public class GenerateThumbImageCommand : IRequest<bool>
    {
        public long AdvertiseId { get; set; }
        public long? MainPhotoId { get; set; }
        public string Path { get; set; }
        public List<long> PhotoAlbumIds { get; set; }
        public GenerateThumbImageCommand(long advertiseId, long? mainPhotoId, List<long> photoAlbumIds, string path)
        {
            AdvertiseId = advertiseId;
            MainPhotoId = mainPhotoId;
            PhotoAlbumIds = photoAlbumIds;
            Path = path;
        }
    }
}
