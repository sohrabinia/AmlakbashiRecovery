using System;
using System.Collections.Generic;
using System.Linq;
using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base
{
    public abstract class AdvertiseBuilderBase : BuilderBase<IPart>
    {
        protected Dictionary<Type, IPart> partDict = new Dictionary<Type, IPart>();
        protected Advertise data;
        protected abstract void BuildParts();
        public AdvertiseBuilderBase(Product<IPart> product) : base(product)
        {
        }

        public void Generate(Advertise data)
        {
            if (data == null)
                data = new Advertise();
            this.data = data;
            BuildParts();
        }

        public bool Validate(out Dictionary<string, string> errors, out List<string> groupErrors)
        {
            var values = partDict.Values;
            var rawValidators = values.Where(w => typeof(IValidator).IsAssignableFrom(w.GetType()));
            var validators = rawValidators.Cast<IValidator>();
            errors = new Dictionary<string, string>();
            groupErrors = new List<string>();
            string msg;
            foreach (var validator in validators)
            {
                Dictionary<string, string> newErrors;
                validator.Validate(out newErrors, out msg);
                if (validator.GetType().Name == "FloorPart" &&
                    !(data.TypeID == Advertise.AdvertiseType.Apartment ||
                    data.TypeID == Advertise.AdvertiseType.SuitAndRoom))
                {
                    continue;
                }
                foreach (var item in newErrors)
                {
                    errors.Add(item.Key, item.Value);
                    if (item.Value != null && !item.Key.Contains("Price"))
                    {
                        groupErrors.Add(item.Value);
                    }
                }
                if (!string.IsNullOrEmpty(msg))
                {
                    groupErrors.Add(msg);
                }
            }
            return errors.Any() == false;
        }

        protected void BuildAdvertisePart<T>() where T : class,IPart, new ()
        {
            T part = new T();
            PropertyCopier<Advertise, T>.Copy(data, part);
            BuildPart(part);
            partDict.Add(typeof(T), part);
        }

        public T GetAdvertisePart<T>() where T : class, IPart
        {
            IPart value;
            if (partDict.TryGetValue(typeof(T), out value))
            {
                return value as T;
            }
            return null;
        }

        public void Submit(ref Advertise advertise)
        {
            foreach (var item in partDict.Values)
            {
                PropertyCopier<IPart, Advertise>.Copy(item, advertise);
            }
        }

        public bool HasImportantChange(Advertise oldAcc)
        {
            foreach (var item in partDict)
            {
                var sourceProperties = item.Value.GetType().GetProperties().Where(a => Attribute.IsDefined(a, typeof(ImportantAttribute)));
                var targetProperties = oldAcc.GetType().GetProperties().Where(w => sourceProperties.Any(a => a.Name.ToLower() == w.Name.ToLower()));
                foreach (var parentProperty in sourceProperties)
                {
                    foreach (var childProperty in targetProperties)
                    {
                        if (parentProperty.Name == childProperty.Name &&
                            parentProperty.PropertyType == childProperty.PropertyType)
                        {
                            var newValue = data.GetType().GetProperty(parentProperty.Name).GetValue(data);
                            var oldValue = oldAcc.GetType().GetProperty(childProperty.Name).GetValue(oldAcc);
                            if ((newValue == null && oldValue != null) || (newValue != null && oldValue == null))
                            {
                                return true;
                            }
                            else if (newValue != null && oldValue != null &&
                                !data.GetType().GetProperty(parentProperty.Name).GetValue(data).
                                Equals(oldAcc.GetType().GetProperty(childProperty.Name).GetValue(oldAcc)))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
