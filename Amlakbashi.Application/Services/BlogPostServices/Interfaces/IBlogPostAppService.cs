using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Entities;
using System.Collections.Generic;
using static Amlakbashi.Core.Entities.BlogPost;

namespace Amlakbashi.Application.Services.BlogPostServices.Interfaces
{
    public interface IBlogPostAppService : IAppService<BlogPost, int>
    {
        BlogPost Find(int id);
        IList<BlogPost> Filter(
            int id,
            SortOrdersEnum sortOrder,
            BlogPostStatus status,
            PlaceEnum showingPlace,
            string postTitle,
            int Province,
            int City,
            int Area);
        IList<BlogPost> GetAll();
        bool Validate(BlogPost data, out string[] errorMessages);
        void Insert(BlogPost item, int userId);
        void Update(BlogPost item, int userId);
        void Scrap(int id);
        void Recycle(int id);
        IList<BlogPost> GetNewItems(PlaceEnum showingPlace, int count);
        IList<BlogPost> GetAccommodationNewItems(int city, int area,
            int accommodationType, int positionType, bool hasPool, int count);
        void Delete(int id);
    }
}
