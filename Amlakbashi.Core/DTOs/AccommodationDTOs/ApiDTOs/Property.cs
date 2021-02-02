using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.ApiDTOs
{
    [Serializable]
    public class Property<T>
    {
        public Property(T value, string title, bool active)
        {
            this.value = value;
            this.title = title;
            this.active = active;
        }
        public T value { get; set; }
        public string title { get; set; }
        public bool active { get; set; }
    }
}
