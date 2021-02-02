using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Infrastructure.DTOHelpers
{
    [Serializable]
    public class DTOCheckbox
    {
        public string name { get; set; }
        public bool? value { get; set; }
        public string title { get; set; }
    }
}
