using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.PaymentDTOs
{
    public class BankResultDTO
    {
        public string TargetCardNumber { get; set; }
        public string CardOwnerName { get; set; }
        public string BankName { get; set; }
        public long Price { get; set; }
        public string SourceDesc { get; set; }
        public string TargetDesc { get; set; }
        public long ReserveId { get; set; }
        public bool success { get; set; }
        public string Description { get; set; }
        public string DateString { get; set; }
        public string TrackingNumber { get; set; }
        public string RefNumber { get; set; }

        public static implicit operator BankResultDTO(List<Cell> cells)
        {
            var item = new BankResultDTO();
            item.TargetCardNumber = cells[0].CellValue.Text;
            item.CardOwnerName = cells[1].CellValue.Text;
            item.BankName = cells[2].CellValue.Text;
            var priceStr = Regex.Replace(cells[3].CellValue.Text, "[^0-9]+", string.Empty);
            item.Price = priceStr.Length > 0 ? long.Parse(priceStr) : 0;
            item.SourceDesc = cells[4].CellValue.Text;
            item.TargetDesc = cells[5].CellValue.Text;
            var reserveIdStr = Regex.Replace(item.TargetDesc, "[^0-9]+", string.Empty);
            item.ReserveId = reserveIdStr.Length > 0 ? long.Parse(reserveIdStr) : 0;
            item.success = cells[6].CellValue.Text == "موفق";
            item.Description = cells[7].CellValue.Text;
            item.DateString = cells[8].CellValue.Text;
            item.TrackingNumber = cells[9].CellValue.Text;
            item.RefNumber = cells[10].CellValue.Text;
            return item;
        }
    }
}
