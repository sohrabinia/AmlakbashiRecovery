using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder;
using Amlakbashi.Core.Entities;
using System.Collections.Generic;
using static Amlakbashi.Core.Entities.Advertise;
using Amlakbashi.Core.DTOs.AccommodationDTOs;
using Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs;
using Amlakbashi.Core.DTOs.AccommodationDTOs.ApiDTOs;
using System.Linq;
using Amlakbashi.Core.DTOs.AccommodationDTOs.CheckDTOs;
using System;
using static Amlakbashi.Core.Entities.ActionLog;
using Amlakbashi.Core.DTOs.AdvertiseDTOs;
using Microsoft.AspNetCore.Http;
using Amlakbashi.Core.DTOs.WebService.Requests.Advertises;
using Amlakbashi.Core.DTOs.WebService.Responses.Advertises;

namespace Amlakbashi.Application.Services.AdvertiseServices.Interfaces
{
    public interface IAdvertiseAppService : IAppService<Advertise, long>
    {
        IQueryable<Advertise> GetAllAsIQueriable();
        AdvertiseListResponse Filter(AdvertisesRequest request);
        IList<Advertise> Filter(string id);
        void FilterNew(AdvertiseIndexDTO dto);
        IList<Advertise> Filter(string statusString, int userid, long id);
        IList<Advertise> FilterAdmin(int province = 0, int city = 0, int area = 0, int adtype = 0,
            bool defaultProvince = false, int adStatus = -1);
        IList<Advertise> FilterAdmin(int province, int city, int area, int adtype, DateTime fromDate, DateTime toDate, int userId);
        IList<Advertise> GetAdvertisesByUserId(int userId, bool includeCommentsAndReports = false);
        IList<long> GetAdvertiseIdsByUserId(int userId);
        IList<Advertise> GetNotChildAdvertisesByUserId(int userId);
        IList<Advertise> GetInstantReserveAdvertisesByUserId(int userId, InstantReserveStatusEnum instantStatus);
        IList<Advertise> GetAdvertisesByStatus(AdvertiseStatus status, bool haveSlug = false);
        IList<Advertise> GetMostLiked(int count, bool beInstantReserve = false);
        List<string> GetAdvertiseTags(Advertise advertise);
        AdvertiseListResponse GetUserFavoriteAdvertises(int userId, int page = 1, int pageItemCount = 20);
        Advertise Find(long id, bool includeOccupiedTables = false);
        Advertise Find(long id, int statusLowerThan);
        Advertise FindIncludingDeleted(long id);
        bool Delete(long id);
        void AddSupporterInfo(long id, string text, User supporter);
        void UpdateAccView(long accId);
        AdvertiseDirector GetAdvertisePageData(long id, out Dictionary<AdvertiseType, IList<AdvertiseDirector>> childrenDirectors);
        void Edit(Advertise editedAd);
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
        PriceInputDTO GetPrices(long id);
        IDictionary<string, DatePriceDTO> GetAccPriceDatesInfo(long id);
        void SetNorouzMinReserveDate(long id, long dateUnix);
        void SetAvailable(long id, bool isAvailable);
        void Publish(long id, int doerUserId, ActionSourceEnum actionSource);
        void Suspend(long id);
        AdvertiseStatus ToggleSuspension(long id);
        void NotVerify(long id, int currentUserId = 0);
        void SetNotVerifyReasons(long id, List<Advertise.NotVerifyReasonsEnum> reasons);
        void SetAsTodayEmpty(long id);
        void UnsetTodayEmpty(long id);
        Dictionary<string, string> GetAdvertiseListPrices(List<long> ids);
        void RequestInstantReserve(long id,
            bool ignoreMsg, int userId,
            int doerUserId, ActionLog.ActionSourceEnum actionSource,
            User.InstantReserveAccessEnum currInstantReserveAccess,
            out bool needMsg);
        void CancelInstantReserve(long id, int userId, int doerUserId,
            ActionLog.ActionSourceEnum actionSource);
        string GetInstantReserveBanReason(long id);
        int GetInstantReserveCancelCount(int userId);
        void SetStayDuration(long id, int min, int max);
        bool SetPrices(long id, PriceInputDTO prices, out Dictionary<string, string> errors);
        void SetNorouzPrice(long id, int norouzPrice, int overCapacityPrice = 0);
        void SetMaxInstantReserveStart(long id, int maxInstantReserveStart);
        void AddToAdvertiseVisit(long id);
        IList<Advertise> GetAdvertiseRelatedItems(long id, int count = 4);
        ApiAmenitiesGetDTO GetAmenitiesDTO(long id, out int userId);
        void UpdateExtraBlanketCount(long id, ExtraBlanketCountItems data);
        void UpdateElevator(long id, bool data);
        bool UpdateAmenities(ApiAmenitiesDTO editedData, out Dictionary<string, string> errors, out string msg);
        ApiPhotoDTO GetPhotoDTO(long id, out int accUserId);
        bool UpdatePhotos(ApiPhotoDTO editedData, string rootPath);
        ApiPositionDTO GetPositionDTO(long id, out int userId);
        bool UpdatePositionDTO(ApiPositionDTO editedData, out Dictionary<string, string> errors);
        ApiRulesDTO GetRulesDTO(long id, out int userId);
        bool UpdateRulesDTO(ApiRulesDTO editedData);
        ApiSpecificDTO GetSpecificDTO(long id, out int userId);
        bool UpdateSpecificDTO(ApiSpecificDTO editedData, bool hasChild, out List<string> errors);
        ApiHotelUnitDTO GetHotelUnitDTO(long id, out int userId);
        bool UpdateHotelUnitDTO(ApiHotelUnitDTO editedData, out List<string> errors);
        ApiNorouzPriceDTO GetNorouzPriceDTO(long id, out int userId);
        void UpdateInstantReserveStatus(int userId, InstantReserveStatusEnum status, bool forRequested = false);
        void UpdateInstantReserveStatus(long accId, InstantReserveStatusEnum status, int doerUserId, ActionSourceEnum actionSource);
        void SetNorouzPrice(long id, int norouzPrice, int overCapacityPrice = 0, int buildNumber = 0);
        IEnumerable<Advertise> GetMostViewedAdvertisesInCity(int city_id, int province_id, int type_id, int count);
        IEnumerable<Advertise> GetMostViewedAdvertisesByType(int type_id, int count);
        IList<Advertise> GetMostViewedNorouzAdvertises(int count);
        IList<Advertise> GetAccListByIds(IList<long> ids, AdvertiseStatus status = AdvertiseStatus.Unset);
        bool IsReserveAvailable(long advertiseId, string fromDate, string toDate, int numberOfGuests,
            out bool isOccupied, out bool guestsOutOfRange, out List<string> occupiedDates);
        IList<string> GetOccupiedDatesInRange(long advertiseId, string persianFrom, string persianTo);
        CheckUnsetOccupiedDTO CheckUnsetOccupiedDateRange(long advertiseId, string from_date, string to_date);
        CheckSetOccupiedDTO CheckSetAsOccupiedDateRange(long advertiseId, string from_date, string to_date);
        bool CheckReserve(int currentUserId, long advertiseId, int guestCount, string startDate, string endDate, out string msg);
        long GetReservePrice(long advertiseId, string startDate, string endDate, int guestCount,
            out long priceWithoutDiscount, out long couponCalculationPrice);
        bool AddAdvertiseComment(int userId, long advertiseId, string text,
            out string cannotAddReason, int? operatorId = null);
        void AddAdvertiseHostReplyComment(int userId, long advertiseId,
            string text, int? operatorId = null);
        Dictionary<string, string> GetRulesDictionary(long id);
        void DeleteExtrinsicReserves(long advertiseId, string from_date, string to_date, bool withLastDay = false);
        bool ReserveRequest(long advertiseId, int userId, string startDate,
            string endDate, int guestCount, bool instantReserve, out string msg, out long reserveId);
        IList<Advertise> GetNorouzAdvertises(int count);
        void SetHygieneProtocol(long id, HygieneProtocolStatus value);
    }
}
