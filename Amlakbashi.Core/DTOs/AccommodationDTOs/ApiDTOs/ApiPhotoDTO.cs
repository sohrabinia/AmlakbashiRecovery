using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.ApiDTOs
{
    public class ApiPhotoDTO
    {
        public long id { get; set; }
        public long mainPhoto { get; set; }
        public List<long> album { get; set; }

        public List<long> ConvertAlbumToList(string album_string)
        {
            var output = new List<long>();
            if (!string.IsNullOrEmpty(album_string) && album_string != ",")
            {
                var album_arr = album_string.Trim(',').Split(',');
                output = Array.ConvertAll(album_arr, x => long.Parse(x)).ToList();
            }
            return output;
        }

        public string ConvertAlbumToString()
        {
            return album.Count == 0 ? "," :
                ("," + string.Join(",", album) + ",");
        }
    }
}
