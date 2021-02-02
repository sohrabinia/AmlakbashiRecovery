using System.Collections.Generic;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base
{
    public interface IValidator
    {
        bool Validate(out Dictionary<string, string> errors, out string msg);
    }
}
