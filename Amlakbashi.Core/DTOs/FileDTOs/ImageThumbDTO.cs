using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.FileDTOs
{
    [Serializable]
    public class ImageThumbDTO
    {
        public string directoryPath { get; set; }
        public string OrigPath { get; set; }
        public string thumbPath { get; set; }
        public int w { get; set; }
        public int h { get; set; }
    }
}
