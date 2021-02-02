using Amlakbashi.Core.Entities;
using System.Collections.Generic;

namespace Amlakbashi.Core.DTOs
{
    public class ServiceDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int ParentId { get; set; }
        public List<ServiceDTO> children { get; set; }
        public Service Parent { get; set; }
        public void AddChildren(List<ServiceDTO> childrenToAdd)
        {
            if (children == null)
                children = new List<ServiceDTO>();
            children.AddRange(childrenToAdd);
        }
    }
}
