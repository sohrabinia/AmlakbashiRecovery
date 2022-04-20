using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amlakbashi.Application.DTOs
{
    public class ServiceResult<T>
    {
        public T Result { get; set; }
        private IList<string> ErrorMessages { get; set; } = new List<string>();

        public void AddError(string error)
        {
            ErrorMessages.Add(error);
        }

        public IList<string> GetErrors()
        {
            return ErrorMessages;
        }

        public bool HasError()
        {
            return ErrorMessages.Any();
        }
    }
}
