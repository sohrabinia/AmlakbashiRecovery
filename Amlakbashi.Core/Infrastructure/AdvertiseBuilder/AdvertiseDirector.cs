using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Entities;
using System.Collections.Generic;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder
{
    public class AdvertiseDirector : GenericDirector<Advertise, AdvertiseBuilderBase>
    {
        public AdvertiseType AdvertiseType { get; private set; }
        public AdvertiseMode Mode { get; private set; }
        public AdvertiseDirector(Advertise data, DirectorType type) : base(data, DirectorData.GenerateBuilder(data, type))
        {
            AdvertiseType = data.TypeID;
            Mode = data.Mode;
            builder.Generate(data);
        }

        public bool Validate(out Dictionary<string, string> errors, out List<string> groupErrors)
        {
            return builder.Validate(out errors, out groupErrors);
        }

        public bool HasImpotantChange(Advertise oldAcc)
        {
            return builder.HasImportantChange(oldAcc);
        }

        public void Submit(ref Advertise advertise)
        {
            builder.Submit(ref advertise);
        }

        public T GetAdvertisePart<T>() where T : class, IPart
        {
            return builder.GetAdvertisePart<T>();
        }
    }
}
