using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Authentication
{
    public interface IUserAccessor
    {
        User CurrentUser { get; }
        User DoerUser { get; }
    }
}
