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
        Tag Find(int id);
        Task<Tag> FindAsync(string title);
        Task GetListAsync(TagListDTO dto);
        Task<IList<Tag>> GetListAsync(string title = null, Tag.TagStatusEnum? status = null);
        Task<ServiceResult> GetTagResidences(TagResidencesDTO dto);
        Task<ServiceResult<Tag>> AddByAdminAsync(string title, 
            Tag.TagStatusEnum status = Tag.TagStatusEnum.Unset);
        Task<ServiceResult<Tag>> AddByUserAsync(long residenceId, string title);
        Task<ServiceResult> UpdateStatusAsync(int id, Tag.TagStatusEnum status);
        Task<ServiceResult> UpdateTitleAsync(int id, string newTitle);
        Task<ServiceResult> DeleteAsync(int id);
    }
}
