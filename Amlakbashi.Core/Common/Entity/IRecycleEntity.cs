using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Common.Entity
{
    public interface IRecycleEntity
    {
        bool IsRecycled { get; set; }
    }
}
