using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder;
using Amlakbashi.Core.Entities;
using System.Collections.Generic;
using static Amlakbashi.Core.Entities.Advertise;
using Amlakbashi.Core.DTOs.AccommodationDTOs;
using Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs;
using System.Linq;
using Amlakbashi.Core.DTOs.AccommodationDTOs.CheckDTOs;
using System;
using static Amlakbashi.Core.Entities.ActionLog;
using Amlakbashi.Core.DTOs.AdvertiseDTOs;
using Microsoft.AspNetCore.Http;
using Amlakbashi.Core.DTOs.WebService.Requests.Advertises;
using Amlakbashi.Core.DTOs.WebService.Responses.Advertises;
using Amlakbashi.Application.DTOs;
using System.Threading.Tasks;

namespace Amlakbashi.Application.Services.AdvertiseServices.Interfaces
{
    public interface IAdvertiseAppService : IAppService<Advertise, long>
    {
        IQueryable<Advertise> GetAllAsIQueriable();
        AdvertiseListResponse Filter(AdvertiseGetListRequest request);
        IList<Advertise> Filter(string id);
        void FilterNew(AdvertiseIndexDTO dto);
        IList<Advertise> Filter(string statusString, int userid, long id);
        IList<Advertise> FilterAdmin(int province = 0, int city = 0, int area = 0, int adtype = 0,
            bool defaultProvince = false, int adStatus = -1);
        IList<Advertise> FilterAdmin(int province, int city, int area, int adtype, DateTime fromDate, DateTime toDate, int userId);
        IList<Advertise> GetAdvertisesByUserId(int userId, bool includeCommentsAndReports = false);
        IList<long> GetAdvertiseIdsByUserId(int userId);
        IList<Advertise> GetAdvertisesByStatus(AdvertiseStatus status, bool haveSlug = false);
        IList<Advertise> GetMostLiked(int count, bool beInstantReserve = false);
        List<string> GetAdvertiseTags(Advertise advertise);
        AdvertiseListResponse GetUserFavoriteAdvertises(int userId, int page = 1, int pageItemCount = 20);
        Advertise Find(long id, bool includeOccupiedTables = false);
        Advertise FindIncludingDeleted(long id);
        bool Delete(long id);
        void AddSupporterInfo(long id, string text, User supporter);
        void UpdateAccView(long accId);
        AdvertiseDirector GetAdvertisePageData(long id, out Dictionary<AdvertiseType, IList<AdvertiseDirector>> childrenDirectors);
        void Edit(Advertise editedAd, int adminId);

        //##############
        Task<ServiceResult<long>> CreateAsync(AdvertisePostCreateRequest request);
        Task<ServiceResult<long>> UpdateBasicInfoAsync(AdvertisePostBasicInfoRequest request);
        Task<ServiceResult> UpdateGeneralInfoAsync(AdvertisePostGeneralInfoRequest request);
        Task<ServiceResult> UpdateSupplementaryInfoAsync(AdvertisePostSupplementaryInfoRequest request);
        Task<ServiceResult> UpdateFinalInfoAsync(AdvertisePostFinalInfoRequest request);
        Task<ServiceResult> CreateHotelRoomAsync(AdvertisePostHotelRoomInfoRequest request);
        Task<ServiceResult> UpdateHotelRoomInfoAsync(AdvertisePostHotelRoomInfoRequest request);
        //##############

        AdvertiseDirector GetBasicForm(long id, out bool isEdit, out int level);
        AdvertiseDirector SubmitBasicForm(Advertise data, int userId, out Dictionary<string, string> errors, out List<string> groupErrors, out int level);
        AdvertiseDirector GetGeneralForm(long id, out bool isEdit, out int level);
        AdvertiseDirector SubmitGeneralForm(Advertise data, out Dictionary<string, string> errors, out List<string> groupErrors, out int level, string rootPath, bool isEdit = false);
        AdvertiseDirector GetExtraForm(long id, out bool isEdit, out int level);
        AdvertiseDirector SubmitExtraForm(Advertise data, out Dictionary<string, string> errors, out List<string> groupErrors, out int level, IFormFile uploadedLicenseFile, bool isEdit = false);
        AdvertiseDirector GetHotelForm(long id, long parentId, out bool isEdit);
        AdvertiseDirector SubmitHotelForm(Advertise data, int userId, out Dictionary<string, string> errors, out List<string> groupErrors, bool save);
        AdvertiseDirector GetComplexForm(long id, long parentId, out AdvertiseType parentType, out bool isEdit);
        AdvertiseDirector SubmitComplexForm(Advertise data, int userId, out Dictionary<string, string> errors,
            out List<string> groupErrors, bool save, out AdvertiseType parentType, string rootPath);
        AdvertiseDirector GetAdminForm(long id, DirectorType type, out AdvertiseType parentType, out AdvertiseStatus status);
        AdvertiseDirector SubmitAdminBasicForm(Advertise data, out Dictionary<string, string> errors,
            out List<string> groupErrors, int currentUserId);
        AdvertiseDirector SubmitAdminForm(Advertise data, out Dictionary<string, string> errors,
            out List<string> groupErrors, bool forceSave, DirectorType type, int currentUserId,
            out AdvertiseType parentType, out AdvertiseStatus status, IFormFile uploadedLicenseFile = null);
        Dictionary<AdvertiseType, Dictionary<long, string>> GetAccChilds(long parentId);
        IDictionary<string, DatePriceDTO> GetAccPriceDatesInfo(long id);
        void SetNorouzMinReserveDate(long id, long dateUnix);
        void SetAvailable(long id, bool isAvailable);
        void Publish(long id, int doerUserId, ActionSourceEnum actionSource);
        void Suspend(long id);
        Task<ServiceResult<AdvertiseStatus>> UpdateActivity(long residenceId);
        void NotVerify(long id, int currentUserId = 0);
        void SetNotVerifyReasons(long id, List<Advertise.NotVerifyReasonsEnum> reasons);
        void SetAsTodayEmpty(long id);
        void UnsetTodayEmpty(long id);
        Dictionary<string, string> GetAdvertiseListPrices(List<long> ids);
        void SetStayDuration(long id, int min, int max);
        void SetNorouzPrice(long id, int norouzPrice, int overCapacityPrice = 0);
        void SetMaxInstantReserveStart(long id, int maxInstantReserveStart);
        IList<Advertise> GetAdvertiseRelatedItems(long id, int count = 4);
        Task<bool> UpdateInstantReserveStatus(long residenceId, InstantReserveStatusEnum status);
        IEnumerable<Advertise> GetMostViewedAdvertisesInCity(int city_id, int province_id, int type_id, int count);
        IList<Advertise> GetMostViewedNorouzAdvertises(int count);
        IList<Advertise> GetAccListByIds(IList<long> ids, AdvertiseStatus status = AdvertiseStatus.Unset);
        bool IsReserveAvailable(long advertiseId, string fromDate, string toDate, int numberOfGuests,
            out bool isOccupied, out bool guestsOutOfRange, out List<string> occupiedDates);
        IList<string> GetOccupiedDatesInRange(long advertiseId, string persianFrom, string persianTo);
        CheckUnsetOccupiedDTO CheckUnsetOccupiedDateRange(long advertiseId, string from_date, string to_date);
        CheckSetOccupiedDTO CheckSetAsOccupiedDateRange(long advertiseId, string from_date, string to_date);
        bool CheckReserve(int currentUserId, long advertiseId, int guestCount, string startDate, string endDate,
            out string msg, out bool isInstantReserve);
        long GetReservePrice(long advertiseId, string startDate, string endDate, int guestCount,
            out long priceWithoutDiscount, out long couponCalculationPrice);
        bool AddAdvertiseComment(int userId, long advertiseId, string text,
            out string cannotAddReason, int? operatorId = null);
        void AddAdvertiseHostReplyComment(int userId, long advertiseId,
            string text, int? operatorId = null);
        Dictionary<string, string> GetRulesDictionary(long id);
        void DeleteExtrinsicReserves(long advertiseId, string from_date, string to_date, bool withLastDay = false);
        bool ReserveRequest(long advertiseId, int userId, string startDate,
            string endDate, int guestCount, out string msg, out long reserveId);
        IList<Advertise> GetNorouzAdvertises(int count);
        void SetHygieneProtocol(long id, HygieneProtocolStatus value);
        void UpdateAlbumPhoto(long advertiseId);
        Task<ServiceResult> UpdatePricesAsync(ResidenceMainPricesDTO request, int adminId = 0);
        Task<ServiceResult> UpdateCalendarAsync(AdvertiseUpdateCalendarRequest request);
        Task<ServiceResult<List<long>>> AddInstantReserveDates(long residenceId, string fromDate, string toDate, int userId);
        Task<ServiceResult<List<long>>> DeleteInstantReserveDates(long residenceId, string fromDate, string toDate, int userId);
        Task UpdateVideoStatus(long residenceId, Advertise.VideoStatusEnum status);
    }
}
