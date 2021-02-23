using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using System.Collections.Generic;
using System.Linq;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts
{
    public class HygieneProtocolPart : IPart, IValidator
    {
        public bool? HygieneProtocol { get; set; }
        public bool Validate(out Dictionary<string, string> errors, out string msg)
        {
            errors = new Dictionary<string, string>();
            if (HygieneProtocol == null)
            {
                errors.Add("HygieneProtocol", "لطفا وضعیت رعایت پروتکل بهداشتی را انتخاب کنید");
            }
            var validated = errors.Any() == false;
            msg = "";
            return validated;
        }
    }
}
