using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.ApiDTOs
{
    [Serializable]
    public class ApiNorouzPriceDTO
    {
        public long id { get; set; }
        public int norouzPrice { get; set; }
        public int norouzOverCapacityPrice { get; set; }
        public string norouzPriceString { get; set; }
        public string norouzOverCapacityPriceStr { get; set; }

        public void SetNorouzPrice(int newPrice)
        {
            this.norouzPrice = newPrice;
            this.norouzPriceString = newPrice == 0 ? "0" :
                string.Format("{0:n0}", newPrice);
        }

        public void SetNorouzOverCapacityPrice(int newPrice)
        {
            this.norouzOverCapacityPrice = newPrice;
            this.norouzOverCapacityPriceStr = newPrice == 0 ? "0" :
                string.Format("{0:n0}", newPrice);
        }

        public bool Validate(out List<string> errors)
        {
            bool has_error = false;
            errors = new List<string>();
            if (this.norouzPrice > 0 && this.norouzPrice < 30000)
            {
                errors.Add("حداقل مبلغ: 30,000 تومان");
                has_error = true;
            }
            if (this.norouzOverCapacityPrice > 0 && this.norouzOverCapacityPrice < 1000)
            {
                errors.Add("حداقل مبلغ مهمان اضافه: 1,000 تومان");
                has_error = true;
            }
            return !has_error;
        }
    }
}
