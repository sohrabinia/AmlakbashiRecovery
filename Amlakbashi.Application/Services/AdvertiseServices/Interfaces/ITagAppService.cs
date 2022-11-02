using Amlakbashi.Application.DTOs;
using Amlakbashi.Core.DTOs.TagDTOs;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Application.Services.AdvertiseServices.Interfaces
{
    public interface ITagAppService
    {
        Task GetListAsync(TagListDTO dto);
        Task<IList<Tag>> GetListAsync(string title);
        Task<ServiceResult<Tag>> AddAsync(string title, Tag.TagStatusEnum status = Tag.TagStatusEnum.Unset);
        Task<ServiceResult> UpdateStatusAsync(int id, Tag.TagStatusEnum status);
        Task<ServiceResult> UpdateTitleAsync(int id, string newTitle);
        Task<ServiceResult> DeleteAsync(int id);
    }
}
