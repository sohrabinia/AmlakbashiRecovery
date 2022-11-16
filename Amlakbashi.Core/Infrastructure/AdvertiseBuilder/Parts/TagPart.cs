using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Entities;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts
{
    public class TagPart : IPart
    {
        public ICollection<Tag> Tags { get; set; }
        public long Id { get; set; }
        public AdvertiseType TypeID { get; set; }
        public Region RegionCity { get; set; }
        public Dictionary<int, string> TagsDic {
            get
            {
                var tagsDic = new Dictionary<int, string>();
                Tags.Each(x => tagsDic.Add(x.Id, x.Title));
                return tagsDic;
            }
        }
    }
}
