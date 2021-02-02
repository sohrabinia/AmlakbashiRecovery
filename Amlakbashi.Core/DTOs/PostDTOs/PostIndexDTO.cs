using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.PostDTOs
{
    public class PostIndexDTO
    {
        public Post Post { get; set; }
        public string UserPhoneNumber { get; set; }
    }
}
