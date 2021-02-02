using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base
{
    public interface IImportantChange
    {
        bool IsImportant(Advertise oldAcc);
    }
}
