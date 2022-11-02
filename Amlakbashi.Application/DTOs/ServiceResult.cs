using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amlakbashi.Application.DTOs
{
    public class ServiceResult
    {
        private IList<string> ErrorMessages = new List<string>();
        public bool CheckHasError => ErrorMessages.Any();
        public IList<string> Errors => ErrorMessages;

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

    public class ServiceResult<T> : ServiceResult
    {
        public T Result { get; set; }
    }
}
