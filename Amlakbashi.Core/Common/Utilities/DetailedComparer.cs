using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;

namespace Amlakbashi.Core.Common.Utilities
{
    public static class DetailedComparer
    {
        public class Variance
        {
            public string Prop { get; set; }
            public object valA { get; set; }
            public object valB { get; set; }
        }

        public static List<Variance> DetailedCompare<T>(string serialized_string_1,
            string serialized_string_2)
        {
            List<Variance> variances = new List<Variance>();
            T val1 = default(T);
            T val2 = default(T);
            if (!string.IsNullOrEmpty(serialized_string_1))
                val1 = JsonConvert.DeserializeObject<T>(serialized_string_1);
            if (!string.IsNullOrEmpty(serialized_string_2))
                val2 = JsonConvert.DeserializeObject<T>(serialized_string_2);

            PropertyInfo[] all_properties = val1 != null ? val1.GetType().GetProperties() :
                val2.GetType().GetProperties();
            if (val1 == null)
            {
                foreach (PropertyInfo p in all_properties)
                {
                    Variance v = new Variance();
                    v.Prop = p.Name;
                    v.valB = p.GetValue(val2);
                    variances.Add(v);
                }
            }
            else if (val2 == null)
            {
                foreach (PropertyInfo p in all_properties)
                {
                    Variance v = new Variance();
                    v.Prop = p.Name;
                    v.valA = p.GetValue(val1);
                    variances.Add(v);
                }
            }
            else
            {
                foreach (PropertyInfo p in all_properties)
                {
                    Variance v = new Variance();
                    v.Prop = p.Name;
                    v.valA = p.GetValue(val1);
                    v.valA = v.valA == null ? "null" : v.valA;
                    v.valB = p.GetValue(val2);
                    v.valB = v.valB == null ? "null" : v.valB;
                    if (!v.valA.Equals(v.valB))
                        variances.Add(v);
                }
            }
            return variances;
        }
    }
}
