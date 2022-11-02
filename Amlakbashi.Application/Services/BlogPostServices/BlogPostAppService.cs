using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using System.Collections.Generic;
using Amlakbashi.Application.Services.BlogPostServices.Interfaces;
using System.Linq;
using static Amlakbashi.Core.Entities.BlogPost;
using System;
using Amlakbashi.Core.Common.Utilities;

namespace Amlakbashi.Application.Services.BlogPostServices
{
    internal class BlogPostAppService : BaseAppService<BlogPost, int>, IBlogPostAppService
    {
        public BlogPostAppService(IRepository<BlogPost, int> repository) : base(repository)
        {
        }

        public BlogPost Find(int id)
        {
            //var item = Cache.Get(id);
            //if (item != null)
            //{
            //    return item;
            //}
            //item = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            //Cache.Set(item);
            //return item;
            return Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
        }

        public IList<BlogPost> Filter(int id = 0,
            SortOrdersEnum sortOrder = SortOrdersEnum.ID_Descending,
            BlogPostStatus status = BlogPostStatus.All,
            PlaceEnum showingPlace = PlaceEnum.Unset, string postTitle = null,
            int Province = 0, int City = 0, int Area = 0)
        {
            var list = Repository.Query(q => q);
            if (id > 0)
            {
                list = list.Where(w => w.Id == id);
            }
            if (status == BlogPostStatus.All)
            {
                list = list.Where(w => w.Status != BlogPostStatus.Scrap);
            }
            else
            {
                list = list.Where(w => w.Status == status);
            }
            if (showingPlace != PlaceEnum.Unset)
            {
                list = list.Where(w => w.ShowingPlace == showingPlace);
            }
            if (!string.IsNullOrEmpty(postTitle))
            {
                list = list.Where(w => w.Title.Contains(postTitle));
            }
            if (Area > 0)
            {
                list = list.Where(w => w.Area == Area);
            }
            else if (City > 0)
            {
                list = list.Where(w => w.City == City);
            }
            else if (Province > 0)
            {
                list = list.Where(w => w.Province == Province);
            }

            switch (sortOrder)
            {
                case SortOrdersEnum.ID_Descending:
                    list = list.OrderByDescending(x => x.Id);
                    break;
                case SortOrdersEnum.ID_Ascending:
                    list = list.OrderBy(x => x.Id);
                    break;
            }
            return list.ToList();
        }

        public bool Validate(BlogPost data, out string[] errorMessages)
        {
            var hasError = false;
            var errorList = new List<string>();
            if (string.IsNullOrEmpty(data.Title))
            {
                hasError = true;
                errorList.Add("لطفا عنوان را وارد کنید");
            }
            if (string.IsNullOrEmpty(data.Text))
            {
                hasError = true;
                errorList.Add("لطفا متن را وارد کنید");
            }
            if (string.IsNullOrEmpty(data.BlogLink))
            {
                hasError = true;
                errorList.Add("لطفا لینک وبلاگ را وارد کنید");
            }
            if (data.PhotoID < 1)
            {
                hasError = true;
                errorList.Add("لطفا یک عکس انتخاب کنید");
            }
            if (data.ShowingPlace == BlogPost.PlaceEnum.Accommodation &&
                data.City < 1)
            {
                hasError = true;
                errorList.Add("لطفا شهر را انتخاب کنید");
            }
            errorMessages = errorList.ToArray();
            return !hasError;
        }

        public void Insert(BlogPost item, int userId)
        {
            item.CreateTime = DateTime.Now;
            item.LastModifyTime = DateTime.Now;
            item.UserID = userId;
            item.LastModifyUserID = userId;
            Repository.Insert(item);
            Repository.Save();
        }

        public void Update(BlogPost item, int userId)
        {
            var currItem = Repository.Query(
                q => q.FirstOrDefault(f => f.Id == item.Id));
            item.CreateTime = currItem.CreateTime;
            item.LastModifyTime = currItem.LastModifyTime;
            item.UserID = currItem.UserID;
            item.LastModifyUserID = userId;
            if (currItem.Equals(item))
                return;
            item.LastModifyTime = DateTime.Now;
            PropertyCopier<BlogPost, BlogPost>.Copy(item, currItem);
            Repository.Update(currItem);
            Repository.Save();
        }

        public void Delete(int id)
        {
            Repository.Delete(id);
            Repository.Save();
        }

        public void Scrap(int id)
        {
            var item = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            item.Status = BlogPostStatus.Scrap;
            Repository.Update(item);
            Repository.Save();
        }

        public void Recycle(int id)
        {
            var item = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            item.Status = BlogPostStatus.Draft;
            Repository.Update(item);
            Repository.Save();
        }

        public IList<BlogPost> GetNewItems(PlaceEnum showingPlace, int count)
        {
            var list = Repository.Query(q => q);
            list = list.Where(w => w.Status == BlogPostStatus.Published);
            list = list.Where(w => w.ShowingPlace == showingPlace);
            if (list.Any())
            {
                list = list.OrderByDescending(x => x.CreateTime).Take(count);
                return list.ToList();
            }
            return new List<BlogPost>();
        }

        public IList<BlogPost> GetAccommodationNewItems(int city, int area,
            int accType, int positionType, bool hasPool, int count)
        {
            var list = Repository.Query(q => q);
            list = list.Where(w => w.Status == BlogPostStatus.Published);
            list = list.Where(w => w.ShowingPlace == PlaceEnum.Accommodation);
            list = list.Where(w => w.City == city);
            if (area > 0)
            {
                list = list.Where(w => w.Area == area || w.Area < 1);
            }
            if (list.Any())
            {
                list = list.
                    OrderByDescending(x => x.Area == area).
                    ThenByDescending(x => hasPool ? (int)x.PoolStatus : 1 - (int)x.PoolStatus).
                    ThenByDescending(x => (int)x.PositionType == positionType).
                    ThenByDescending(x => (int)x.AccommodationType == accType)
                    .Take(count);
            }
            return list.ToList();
        }

        public IList<BlogPost> GetAll()
        {
            return Repository.Query(q => q).ToList();
        }
    }
}
