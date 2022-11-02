using Amlakbashi.Application.DTOs;
using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Extensions;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.DTOs.TagDTOs;
using Amlakbashi.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Application.Services.AdvertiseServices
{
    class TagAppService : BaseAppService<Tag, int>, ITagAppService
    {
        public TagAppService(IRepository<Tag, int> repository) : base(repository)
        {
        }

        public async Task<Tag> FindAsync(int id)
        {
            return await Repository.FindAsync(id);
        }

        public async Task<Tag> FindAsync(string title)
        {
            return await Repository.Query(q => q.FirstOrDefaultAsync(x => x.Title == title));
        }

        public async Task GetListAsync(TagListDTO dto)
        {
            var tags = Repository.Query(q => q);
            if (string.IsNullOrEmpty(dto.title) == false)
            {
                tags = tags.Where(x => x.Title.Contains(dto.title));
            }
            if (dto.status.HasValue)
            {
                tags = tags.Where(x => x.Status == dto.status);
            }
            dto.pagedList = await tags.OrderByDescending(x => x.CreateDate)
                .ToPagedListAsync(dto.page, dto.pageItemCount);
        }

        public async Task<IList<Tag>> GetListAsync(string title)
        {
            return await Repository.Query(q => q.Where(x => x.Title.Contains(title))).ToListAsync();
        }

        public async Task<ServiceResult<Tag>> AddAsync(string title,
            Tag.TagStatusEnum status = Tag.TagStatusEnum.Unset)
        {
            var serviceResult = new ServiceResult<Tag>();
            if (StringUtility.VerifyTagTitle(title) == false)
            {
                serviceResult.AddError("عنوان وارد شده اشتباه است");
                return serviceResult;
            }
            title = title.Trim();
            var tag = await FindAsync(title);
            if (tag != null)
            {
                serviceResult.AddError("عنوان وارد شده تکراری است");
                return serviceResult;
            }

            tag = new Tag()
            {
                Title = title,
                Status = status,
                CreateDate = DateTime.Now
            };
            await Repository.InsertAsync(tag);
            await Repository.SaveAsync();
            serviceResult.Result = tag;
            return serviceResult;
        }

        public async Task<ServiceResult> UpdateStatusAsync(int id, Tag.TagStatusEnum status)
        {
            var serviceResult = new ServiceResult();
            var tag = await FindAsync(id);
            if (tag is null)
            {
                serviceResult.AddError("تگ اشتباه است");
                return serviceResult;
            }

            tag.Status = status;
            Repository.Update(tag);
            await Repository.SaveAsync();
            return serviceResult;
        }

        public async Task<ServiceResult> UpdateTitleAsync(int id, string newTitle)
        {
            var serviceResult = new ServiceResult();
            if (StringUtility.VerifyTagTitle(newTitle) == false)
            {
                serviceResult.AddError("عنوان وارد شده اشتباه است");
                return serviceResult;
            }
            newTitle = newTitle.Trim();
            var tag = await FindAsync(id);
            if (tag is null)
            {
                serviceResult.AddError("تگ اشتباه است");
                return serviceResult;
            }
            if (await FindAsync(newTitle) != null)
            {
                serviceResult.AddError("عنوان وارد شده تکراری است");
                return serviceResult;
            }

            tag.Title = newTitle;
            Repository.Update(tag);
            await Repository.SaveAsync();
            return serviceResult;
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var serviceResult = new ServiceResult<Tag>();
            var tag = await FindAsync(id);
            if (tag is null)
            {
                serviceResult.AddError("تگ انتخاب شده اشتباه است");
                return serviceResult;
            }
            await Repository.DeleteAsync(id);
            await Repository.SaveAsync();
            return serviceResult;
        }
    }
}
