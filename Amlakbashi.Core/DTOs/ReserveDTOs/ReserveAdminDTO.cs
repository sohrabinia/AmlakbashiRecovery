using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using Amlakbashi.Core.Infrastructure.StyleHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Amlakbashi.Core.Entities.Reserve;
using static Amlakbashi.Core.Entities.ReserveSupport;

namespace Amlakbashi.Core.DTOs.ReserveDTOs
{
    public class ReserveAdminDTO
    {
        public List<ReserveAdminItemDTO> reserveList;
    }
}
