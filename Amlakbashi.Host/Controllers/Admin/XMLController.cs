using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using static Amlakbashi.Core.Entities.Region;
using Amlakbashi.Application.Services.Category.Interfaces;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using Amlakbashi.Host.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Amlakbashi.Core.Common.Utilities;

namespace MVC_RSS_Sitemap.Controllers
{
    public class XMLController : BaseController
    {
        private readonly IRegionAppService regionService;
        private readonly ICategoryAppService dynamicCategoryService;
        private readonly IAdvertiseAppService advertiseService;
        public XMLController(IRegionAppService regionService,
            ICategoryAppService dynamicCategoryService,
            IAdvertiseAppService advertiseService)
        {
            this.dynamicCategoryService = dynamicCategoryService;
            this.regionService = regionService;
            this.advertiseService = advertiseService;
        }

        //public ContentResult RSS()
        //{

        //    var items = GetRssFeed();
        //    var rss = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
        //      new XElement("rss",
        //        new XAttribute("version", "2.0"),
        //          new XElement("channel",
        //            new XElement("title", "آخرین مطالب سایت"),
        //            new XElement("link", "http://" + Request.Url.Host + "/rss"),
        //            new XElement("description", "آخرین مطالب سایت من"),
        //            new XElement("copyright", "(c)" + DateTime.Now.Year + ", نام سایت من.تمامی حقوق محفوظ است"),
        //          from item in items
        //          select
        //          new XElement("item",
        //            new XElement("title", item.title),
        //            new XElement("description", item.description),
        //            new XElement("link", item.link),
        //            new XElement("pubDate", item.pubDate)

        //          )
        //        )
        //      )
        //    );
        //    return Content(rss.ToString(), "text/xml");
        //}


        public ContentResult Sitemap()
        {
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            string url_base = Request.Scheme + "://" + Request.Host;
            var sitemap = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(ns + "sitemapindex",
                    new XElement("sitemap",
                      new XElement("loc", url_base + "/advertises-sitemap.xml")
                      ),
                    new XElement("sitemap",
                      new XElement("loc", url_base + "/province-sitemap.xml")
                      ),
                    new XElement("sitemap",
                      new XElement("loc", url_base + "/city-sitemap.xml")
                      )
                    //new XElement("sitemap",
                    //  new XElement("loc", url_base + "/old-sitemap.xml")
                    //  )
                    //new XElement("sitemap",
                    //  new XElement("loc", url_base + "/area-sitemap.xml")
                    //  ),
                    //new XElement("sitemap",
                    //  new XElement("loc", url_base + "/image-sitemap.xml")
                    //  )
                    )
                  );

            string str = sitemap.ToString().Replace("xmlns=\"\"", "");
            return Content(str, "text/xml");
        }


        public ContentResult AdSitemap()
        {
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            string url_base = Request.Scheme + "://" + Request.Host;
            var items = GetLinks();
            var sitemap = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(ns + "urlset",
                    from item in items
                    select
                    new XElement("url",
                      new XElement("loc", item.link),
                      new XElement("lastmod", item.lastmod),
                      new XElement("priority", item.priority)
                      )
                    )
                  );

            string str = sitemap.ToString().Replace("xmlns=\"\"", "");
            return Content(str, "text/xml");
        }

        public ContentResult ImageSitemap()
        {
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XNamespace nsImage = "http://www.google.com/schemas/sitemap-image/1.1";
            string url_base = Request.Scheme + "://" + Request.Host;
            var items = GetImageLinks();
            var sitemap = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(ns + "urlset",
                new XAttribute(XNamespace.Xmlns + "image", nsImage.NamespaceName),
                    from item in items
                    select
                    new XElement("url",
                      new XElement("loc", item.link),
                      new XElement(nsImage + "image",
                              new XElement(nsImage + "loc", item.imagelink),
                              new XElement(nsImage + "geo_location", item.geolocation),
                              new XElement(nsImage + "title", item.title)
                          )
                      )
                    )
                  );

            string str = sitemap.ToString().Replace("xmlns=\"\"", "");
            return Content(str, "text/xml");
        }


        public ContentResult ProvinceSitemap()
        {
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var items = GetProvinces();
            var sitemap = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(ns + "urlset",
                    from item in items
                    select
                    new XElement("url",
                      new XElement("loc", item.link),
                      new XElement("lastmod", item.lastmod),
                      new XElement("priority", item.priority)
                      )
                    )
                  );

            string str = sitemap.ToString().Replace("xmlns=\"\"", "");
            return Content(str, "text/xml");
        }


        public ContentResult CitySitemap()
        {
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var items = GetCities();
            var sitemap = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(ns + "urlset",
                    from item in items
                    select
                    new XElement("url",
                      new XElement("loc", item.link),
                      new XElement("lastmod", item.lastmod),
                      new XElement("priority", item.priority)
                      )
                    )
                  );

            string str = sitemap.ToString().Replace("xmlns=\"\"", "");
            return Content(str, "text/xml");
        }


        public ContentResult AreaSitemap()
        {
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var items = GetArea();
            var sitemap = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(ns + "urlset",
                    from item in items
                    select
                    new XElement("url",
                      new XElement("loc", item.link),
                      new XElement("lastmod", item.lastmod),
                      new XElement("priority", item.priority)
                      )
                    )
                  );

            string str = sitemap.ToString().Replace("xmlns=\"\"", "");
            return Content(str, "text/xml");
        }

        public ContentResult OldSiteMap()
        {
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var items = OldGetProvinces().Concat(OldGetCities()).Concat(OldGetArea());
            var sitemap = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(ns + "urlset",
                    from item in items
                    select
                    new XElement("url",
                      new XElement("loc", item.link),
                      new XElement("lastmod", item.lastmod)
                      )
                    )
                  );

            string str = sitemap.ToString().Replace("xmlns=\"\"", "");
            return Content(str, "text/xml");
        }

        public async Task<ContentResult> TagSitemap([FromServices] ITagAppService tagService)
        {
            string url_base = Request.Scheme + "://" + Request.Host;
            var tags = await tagService.GetListAsync(status: Tag.TagStatusEnum.Active);
            var items = tags.Select(x => new PostToXML()
            {
                title = x.Title,
                lastmod = x.CreateDate.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                priority = "0.7",
                link = $"{url_base}/tag/{StringUtility.GetTagUrlTitle(x.Title)}"
            }).ToList();
            return GenerateSitemap(items);
        }

        private ContentResult GenerateSitemap(IEnumerable<PostToXML> items)
        {
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var sitemap = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(ns + "urlset",
                    from item in items
                    select
                      new XElement("url",
                      new XElement("loc", item.link),
                      new XElement("lastmod", item.lastmod),
                      new XElement("priority", item.priority)
                      )
                    )
                  );

            string str = sitemap.ToString().Replace("xmlns=\"\"", "");
            return Content(str, "text/xml");
        }

        public IEnumerable<PostToXML> GetLinks()
        {
            string url_base = Request.Scheme + "://" + Request.Host;
            var postsFromDb = advertiseService.GetAdvertisesByStatus(Advertise.AdvertiseStatus.Published);

            List<PostToXML> sampleposts = (from p in postsFromDb
                                           select new PostToXML()
                                           {
                                               title = p.Title,
                                               lastmod = p.LastModifiedDate.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                                               priority = "0.6",
                                               link = url_base + string.Format("/{0}/{1}", AdvertiseMainLocalization.CategoryTitle, p.Slug),

                                           }).ToList();

            return sampleposts;
        }

        public IEnumerable<PostToXML> GetImageLinks()
        {
            string url_base = Request.Scheme + "://" + Request.Host;
            var PostsFromDb = advertiseService.GetAdvertisesByStatus(Advertise.AdvertiseStatus.Published, true);
            var provinces = regionService.Filter(AdvertiseRegion.Province);
            List<PostToXML> sampleposts = (from p in PostsFromDb
                                           select new PostToXML()
                                           {
                                               title = p.Title.Replace("-", " "),
                                               imagelink = url_base + string.Format("/عکس-آگهی-بزرگ/{0}", p.Slug),
                                               geolocation = provinces.FirstOrDefault(x => x.Id == p.ProvinceId).EnglishName + ", Iran",
                                               link = url_base + string.Format("/{0}/{1}", AdvertiseMainLocalization.CategoryTitle, p.Slug),

                                           }).ToList();

            return sampleposts;
        }

        public IEnumerable<PostToXML> GetProvinces()
        {
            string url_base = Request.Scheme + "://" + Request.Host;
            var cats = dynamicCategoryService.GetProvincesForXML(false);
            List<PostToXML> sampleposts = (from cat in cats
                                           select new PostToXML()
                                           {
                                               title = cat.Title,
                                               lastmod = cat.LastModifyDate.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                                               priority = "0.8",
                                               link = url_base + CategoryUrlLocalization.CategoryToUrl(cat)
                                           }).ToList();

            return sampleposts;
        }

        public IEnumerable<PostToXML> GetCities()
        {
            string url_base = Request.Scheme + "://" + Request.Host;
            var cats = dynamicCategoryService.GetCitiesForXML(false);
            List<PostToXML> sampleposts = (from cat in cats
                                           select new PostToXML()
                                           {
                                               title = cat.Title,
                                               lastmod = cat.LastModifyDate.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                                               priority = cat.CountAdvertise > 60 ? "1.0" : "0.9",
                                               link = url_base + CategoryUrlLocalization.CategoryToUrl(cat)
                                           }).ToList();

            return sampleposts;
        }

        public IEnumerable<PostToXML> GetArea()
        {
            string url_base = Request.Scheme + "://" + Request.Host;
            var cats = dynamicCategoryService.GetAreasForXML(true);
            List<PostToXML> sampleposts = (from cat in cats
                                           select new PostToXML()
                                           {
                                               title = cat.Title,
                                               lastmod = cat.LastModifyDate.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                                               priority = cat.CountAdvertise > 30 ? "0.9" : "0.8",
                                               link = url_base + CategoryUrlLocalization.CategoryToUrl(cat)
                                           }).ToList();

            return sampleposts;
        }

        public IEnumerable<PostToXML> OldGetProvinces()
        {
            string url_base = Request.Scheme + "://" + Request.Host;
            var cats = dynamicCategoryService.GetProvincesForXML(true);
            List<PostToXML> sampleposts = (from cat in cats
                                           select new PostToXML()
                                           {
                                               title = cat.Title,
                                               lastmod = cat.LastModifyDate.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                                               link = url_base + string.Format("/{0}/{1}", AdvertiseMainLocalization.CategoryTitle, cat.URL)
                                           }).ToList();

            return sampleposts;
        }

        public IEnumerable<PostToXML> OldGetCities()
        {
            string url_base = Request.Scheme + "://" + Request.Host;
            var cats = dynamicCategoryService.GetCitiesForXML(true);
            List<PostToXML> sampleposts = (from cat in cats
                                           select new PostToXML()
                                           {
                                               title = cat.Title,
                                               lastmod = cat.LastModifyDate.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                                               link = url_base + string.Format("/{0}/{1}", AdvertiseMainLocalization.CategoryTitle, cat.URL)
                                           }).ToList();

            return sampleposts;
        }

        public IEnumerable<PostToXML> OldGetArea()
        {
            string url_base = Request.Scheme + "://" + Request.Host;
            var cats = dynamicCategoryService.GetAreasForXML(true);
            List<PostToXML> sampleposts = (from cat in cats
                                           select new PostToXML()
                                           {
                                               title = cat.Title,
                                               lastmod = cat.LastModifyDate.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                                               link = url_base + string.Format("/{0}/{1}{2}", AdvertiseMainLocalization.CategoryTitle, cat.URL, string.IsNullOrEmpty(cat.AreaStr) ? "" : "/" + cat.AreaStr)
                                           }).ToList();

            return sampleposts;
        }

    }

    public class PostToXML
    {
        public string title { get; set; }
        public string link { get; set; }
        public string lastmod { get; set; }
        public string priority { get; set; }
        public string imagelink { get; set; }
        public string geolocation { get; set; }
    }
}