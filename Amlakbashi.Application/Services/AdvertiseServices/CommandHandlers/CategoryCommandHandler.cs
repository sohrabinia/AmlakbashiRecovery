using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using Amlakbashi.Mediator.Commands.AdvertiseCommands;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Amlakbashi.Core.Entities.Advertise;
using static Amlakbashi.Core.Entities.Region;

namespace Amlakbashi.Application.Services.AdvertiseServices.CommandHandlers
{
    public class CategoryCommandHandler :
        IRequestHandler<UpdateCategoryMostAccCommand>,
        IRequestHandler<UpdateCategoryPriceCommand>,
        IRequestHandler<UpdateCategoryAccCountCommand>,
        IRequestHandler<GetCategoriesFilterCommand, List<DynamicCategory>>
    {
        private readonly IRepository<DynamicCategory, int> categoryRepository;
        public CategoryCommandHandler(IRepository<DynamicCategory, int> categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }
        public Task<Unit> Handle(UpdateCategoryPriceCommand request, CancellationToken cancellationToken)
        {
            var cat = categoryRepository.Find(request.categoryId);
            cat.MinPrice = cat.Advertises == null || cat.Advertises.Any() == false ? 40000 :
                cat.Advertises.Min(m => m.BasePrice);
            cat.MaxPrice = cat.Advertises == null || cat.Advertises.Any() == false ? 2000000 :
                cat.Advertises.Max(m => m.BasePrice);
            categoryRepository.Update(cat);
            categoryRepository.Save();
            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(UpdateCategoryMostAccCommand request, CancellationToken cancellationToken)
        {
            var cat = categoryRepository.Find(request.categoryId);
            var accs = cat.Advertises.Where(w => w.TypeID == Advertise.AdvertiseType.Apartment ||
                w.TypeID == Advertise.AdvertiseType.Villa);
            if (accs.Any())
            {
                cat.MostAccType = (int)accs.
                    GroupBy(g => g.TypeID).
                    OrderByDescending(o => o.Count()).FirstOrDefault().Key;
                categoryRepository.Update(cat);
            }
            else
            {
                cat.MostAccType = 0;
                categoryRepository.Update(cat);
            }
            categoryRepository.Save();
            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(UpdateCategoryAccCountCommand request, CancellationToken cancellationToken)
        {
            var cat = categoryRepository.Find(request.categoryId);
            cat.CountAdvertise = cat.Advertises == null ? 0 : cat.Advertises.Count();
            categoryRepository.Update(cat);
            categoryRepository.Save();
            return Task.FromResult(Unit.Value);
        }

        public Task<List<DynamicCategory>> Handle(GetCategoriesFilterCommand request, CancellationToken cancellationToken)
        {
            var type = (AdvertiseType)AdvertiseTypeToHeadType((int)request.Type);
            List<DynamicCategory> categoryList = new List<DynamicCategory>();
            var allCategories = categoryRepository.Query(q => q);

            //add categories which location is iran and their type is all
            categoryList.AddRange(allCategories.Where(w =>
                    w.Type == AdvertiseType.All &&
                    w.CountryDirection == CountryDirection.Unset &&
                    w.Province == null).ToList());

            //add categories which location is iran and their type is same as the advertise
            categoryList.AddRange(allCategories.Where(w =>
                   w.Type == type &&
                   w.CountryDirection == CountryDirection.Unset &&
                   w.Province == null).ToList());

            //if the advertise's country direction is not unset
            if (request.CountryDirection > CountryDirection.Unset)
            {
                //add categories which country direction is same as the advertise and type is all
                categoryList.AddRange(allCategories.Where(w =>
                    w.Type == AdvertiseType.All &&
                    w.CountryDirection == request.CountryDirection &&
                    w.Province == null).ToList());

                //add categories which country direction and type is same as the advertise
                categoryList.AddRange(allCategories.Where(w =>
                    w.Type == type &&
                    w.CountryDirection == request.CountryDirection &&
                    w.Province == null).ToList());
            }

            //add categories which province is same as the advertise and type is all
            categoryList.AddRange(allCategories.Where(w =>
                    w.Type == AdvertiseType.All &&
                    w.Province == request.Province &&
                    w.City == null).ToList());

            //add categories which province and type is same as the advertise
            categoryList.AddRange(allCategories.Where(w =>
                    w.Type == type &&
                    w.Province == request.Province &&
                    w.City == null).ToList());

            //add categories which city is same as the advertise and type is all
            categoryList.AddRange(allCategories.Where(w =>
                    w.Type == AdvertiseType.All &&
                    w.City == request.City &&
                    w.Area == null).ToList());

            //add categories which city and type is same as the advertise
            categoryList.AddRange(allCategories.Where(w =>
                    w.Type == type &&
                    w.City == request.City &&
                    w.Area == null).ToList());

            //if (request.Save)
            //{
            //    foreach (var cat in categoryList)
            //    {
            //        cat.LastModifyDate = DateTime.Now;
            //        categoryRepository.Update(cat);
            //    }
            //    categoryRepository.Save();

            //}

            return Task.FromResult(categoryList);
        }
    }
}
