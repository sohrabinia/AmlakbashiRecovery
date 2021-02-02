using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Infrastructure.DTOHelpers
{
    [Serializable]
    public class DTOSelectItem
    {
        public int value { get; set; }
        public string title { get; set; }

        public DTOSelectItem(int value, string title)
        {
            this.value = value;
            this.title = title;
        }
    }
}
