using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.DTOs.WebService.Responses.Advertises;
using Amlakbashi.Core.DTOs.WebService.Responses.Advertises.AdvertiseParts;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Responses.User
{
    public class HostProfileResponse
    {
        public int id { get; set; }
        public string hostName { get; set; }
        public string hostCreateDate { get; set; }
        public float hostReponseRate { get; set; }
        public string imageUrl { get; set; }
        public int residencyCount { get; set; }
        public List<AdvertiseListItemResponse> residencies { get; set; }
        public int commentCount { get; set; }
        public List<HostProfileCommentsItemResponse> comments { get; set; }

        public static implicit operator HostProfileResponse(Entities.User user)
        {
            var response = new HostProfileResponse()
            {
                id = user.Id,
                hostName = user.FullName,
                hostReponseRate = ((float)user.HostReserves.Where(x => x.HostResponse != Reserve.HostResponseEnum.None).Count()
                / (float)user.HostReserves.Count) * 100,
                imageUrl = user.PhotoID == null ? "" : $"/عکس-پروفایل_کوچک-{user.PhotoID}"
            };
            var publishedAdvertises = user.Advertises.Where(x => x.Status == Advertise.AdvertiseStatus.Published);
            response.residencies = publishedAdvertises.Select(s => (AdvertiseListItemResponse)s).ToList();
            response.residencyCount = response.residencies.Count;
            response.comments = publishedAdvertises.SelectMany(s => s.PublishedComments().Select(c => new HostProfileCommentsItemResponse()
            {
                comment = c.Text,
                date = StringUtility.EnglishNumberToPersian(DateTimeUtility.ConvertDate(c.CreateDate)),
                name = c.SenderUser.FullName,
                imageUrl = c.SenderUser.PhotoID == null ? "" : $"/عکس-پروفایل_کوچک-{c.SenderUser.PhotoID}",
                residencyId = s.Id,
                residencyTitle = s.Title,
                residencyImageUrl = s.PhotoID == null ? "" : $"/file/accthumbxxxlarge?accid={s.Id}&fileid={s.PhotoID}"
            })).ToList();
            response.commentCount = response.comments.Count;
            return response;
        }
    }

    public class HostProfileCommentsItemResponse : AdvertiseCommentItemResponse
    {
        public long residencyId { get; set; }
        public string residencyTitle { get; set; }
        public string residencyImageUrl { get; set; }
    }
}
