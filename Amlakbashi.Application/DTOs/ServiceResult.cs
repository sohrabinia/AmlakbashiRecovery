using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Application.DTOs
{
    public class ServiceResult<T>
    {
        public T Result { get; set; }
        public bool IsValid { get; set; } = true;
        public IList<string> ErrorMessages { get; set; } = new List<string>();
    }
}
