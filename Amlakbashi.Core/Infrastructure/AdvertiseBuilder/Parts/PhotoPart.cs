using Amlakbashi.Core.Base.Builder;
using System.Collections.Generic;
using System.Linq;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts
{
    public class PhotoPart : IPart
    {
        public long? MainPhotoId { get; set; }

        [Important]
        public string AlbumPhoto { get; set; }

        public long[] AlbumPhotosArray
        {
            get
            {
                if (string.IsNullOrEmpty(AlbumPhoto) || AlbumPhoto == ",")
                    return new long[] { };
                var stringArray = AlbumPhoto.Trim(',').Split(',');
                var list = new List<long>();
                long id;
                foreach (var stringId in stringArray)
                {
                    if (long.TryParse(stringId, out id) && id > 0)
                    {
                        list.Add(id);
                    }
                }
                return list.ToArray();
            }
            set
            {
                var list = value.Where(w => w > 0);
                if (list.Any() == false)
                {
                    AlbumPhoto = ",";
                }
                else
                {
                    AlbumPhoto = "";
                    foreach (var item in list)
                    {
                        AlbumPhoto += ("," + item);
                    }
                    AlbumPhoto += ",";
                }
            }
        }
    }
}
