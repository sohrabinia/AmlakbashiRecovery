using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.ApiDTOs
{
    [Serializable]
    public class SelectItem
    {
        public SelectItem(int id, string title)
        {
            this.id = id;
            this.title = title;
        }
        public int id { get; set; }
        public string title { get; set; }
    }
}
