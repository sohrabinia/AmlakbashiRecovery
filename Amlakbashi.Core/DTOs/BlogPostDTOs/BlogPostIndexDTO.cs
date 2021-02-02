using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.BlogPostDTOs
{
    public class BlogPostIndexDTO
    {
        public BlogPost BlogPost { get; set; }
        public string UserFullName { get; set; }
        public string LastModifyUserFullName { get; set; }
        public string ShowingPlace { get; set; }
    }
}
