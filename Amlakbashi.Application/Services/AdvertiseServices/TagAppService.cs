using Amlakbashi.Application.DTOs;
using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Extensions;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.DTOs.AccommodationDTOs;
using Amlakbashi.Core.DTOs.TagDTOs;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
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

        public Tag Find(int id)
        {
            return Repository.Find(id);
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

        public async Task<IList<Tag>> GetListAsync(string title = null, 
            Tag.TagStatusEnum? status = null)
        {
            var tags = Repository.Query(q => q);
            if (string.IsNullOrEmpty(title) == false)
            {
                tags = tags.Where(x => x.Title.Contains(title));
            }
            if (status.HasValue)
            {
                tags = tags.Where(x => x.Status == status.Value);
            }
            return await tags.ToListAsync();
        }

        public async Task<ServiceResult> GetTagResidences(TagResidencesDTO dto)
        {
            var serviceResult = new ServiceResult();
            var tag = await FindAsync(dto.title);
            if (tag is null || tag.Status != Tag.TagStatusEnum.Active)
            {
                serviceResult.AddError("تگ مورد نظر یافت نشد");
                return serviceResult;
            }
            dto.pagedList = tag.Residences.Where(x=>x.Status == Advertise.AdvertiseStatus.Published)
                .Select(x => (AccommodationCardDTO)x)
                .ToPagedList(dto.page, dto.pageItemCount);

            dto.similarTags = await GetSimilarTags(tag, 4);
            return serviceResult;
        }

        private async Task<IList<Tag>> GetSimilarTags(Tag tag, int count)
        {
            var similarTags = await GetListAsync(tag.Title.Split(' ').Last(), Tag.TagStatusEnum.Active);
            similarTags.Remove(tag);
            return similarTags.OrderBy(x => new Random().Next()).Take(count).ToList();
        }

        public async Task<ServiceResult<Tag>> AddByAdminAsync(string title,
            Tag.TagStatusEnum status = Tag.TagStatusEnum.Unset)
        {
            var serviceResult = new ServiceResult<Tag>();
            if (StringUtility.VerifyTagTitle(title) == false)
            {
                serviceResult.AddError("عنوان وارد شده اشتباه است");
                return serviceResult;
            }
            StringUtility.ModifySentence(ref title);
            var tag = await FindAsync(title);
            if (tag != null)
            {
                serviceResult.AddError("عنوان وارد شده تکراری است");
                return serviceResult;
            }
            serviceResult.Result = await AddAsync(title, status);
            return serviceResult;
        }

        public async Task<ServiceResult<Tag>> AddByUserAsync(long residenceId, string title)
        {
            var serviceResult = new ServiceResult<Tag>();
            if (StringUtility.VerifyTagTitle(title) == false)
            {
                serviceResult.AddError("عنوان وارد شده اشتباه است");
                return serviceResult;
            }
            var residence = Repository.Find<Advertise, long>(residenceId);
            if (residence == null)
            {
                serviceResult.AddError("اقامتگاه یافت نشد");
                return serviceResult;
            }
            StringUtility.ModifySentence(ref title);
            title = $"{AdvertiseMainLocalization.GetAdvertiseTypePersianNameForUser(residence.TypeID)} {title} در {residence.RegionCity.PersianName}";
            var tag = await FindAsync(title);
            if (tag != null)
            {
                serviceResult.AddError("عنوان وارد شده تکراری است");
                return serviceResult;
            }
            serviceResult.Result = await AddAsync(title);
            return serviceResult;
        }

        private async Task<Tag> AddAsync(string title, Tag.TagStatusEnum status = Tag.TagStatusEnum.Unset)
        {
            var tag = new Tag()
            {
                Title = title,
                Status = status,
                CreateDate = DateTime.Now
            };
            await Repository.InsertAsync(tag);
            await Repository.SaveAsync();
            return tag;
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
            StringUtility.ModifySentence(ref newTitle);
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
