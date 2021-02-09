using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Authentication
{
    interface IUserAccessor
    {
        User CurrentUser { get; }
        User DoerUser { get; }
    }
}
