using Amlakbashi.Accounting;
using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Application.Services.ReserveServices.Interfaces;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.DTOs.PaymentDTOs;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.UserContact;
using Amlakbashi.Host.Authentication;
using Amlakbashi.Host.Controllers.Base;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using log4net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using X.PagedList;

namespace Amlakbashi.Host.Controllers
{
    public class GroupPaymentController : BaseController
    {
        private readonly IBankCardAppService bankCardService;
        private readonly IUserAppService userService;
        private readonly IAccountingFacade accounting;
        private readonly IReserveAppService reserveService;
        private readonly IAdvertiseAppService advertiseService;
        private readonly IWebHostEnvironment host;
        private readonly IUserAccessor userAccessor;
        private readonly ILog logger;
        public GroupPaymentController(IBankCardAppService bankCardService,
            IUserAppService userService,
            IAccountingFacade accounting,
            IReserveAppService reserveService,
            IAdvertiseAppService advertiseService,
            IWebHostEnvironment host,
            IUserAccessor userAccessor,
            ILog logger)
        {
            this.bankCardService = bankCardService;
            this.userService = userService;
            this.accounting = accounting;
            this.reserveService = reserveService;
            this.advertiseService = advertiseService;
            this.host = host;
            this.logger = logger;
            this.userAccessor = userAccessor;
        }

        [Auth(UserRoles.Admin)]
        public ActionResult Index(int? page, int status = -1)
        {
            try
            {
                var model = accounting.FilterGroupPayment(status);
                List<Reserve> todayPayments, paymentsWithError, excludingPayments;
                var all = accounting.GetGroupPaymentReserves(out todayPayments, out paymentsWithError, out excludingPayments);
                ViewBag.totalPaymentsCount = all.Count();
                ViewBag.todayPaymentsCount = todayPayments.Count;
                ViewBag.paymentsWithErrorCount = paymentsWithError.Count;
                ViewBag.excludingPaymentsCount = excludingPayments.Count;
                ViewBag.status = status;
                ViewBag.msg = TempData["msg"];
                ViewBag.done = TempData["done"];
                var PageNumber = page ?? 1;
                var onePageOfModel = model.ToPagedList(PageNumber, 20);
                ViewBag.RowIndexStart = (PageNumber * 20) - 20;
                return View(onePageOfModel);
            }
            catch (Exception exc)
            {
                logger.Error("GroupPayment.Index", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Auth(UserRoles.Admin)]
        public JsonResult GenerateGroupPay()
        {
            try
            {
                IQueryable<User> users = userService.GetAllAsIQueryable();
                var reservePayments = accounting.GetAllReservePayments();
                List<Reserve> todayPayments, paymentsWithError, excludingPayments;
                accounting.GetGroupPaymentReserves(out todayPayments, out paymentsWithError, out excludingPayments);
                const long maxPrice = 3000000;
                long currentPrice = 0;
                List<Reserve> reserves = new List<Reserve>();
                for (int i = 0; i < todayPayments.Count; i++)
                {
                    var item = todayPayments[i];
                    var hostPayablePrice = PriceUtility.CalculateHostPayablePrice(
                        item.TotalPrice, accounting.GetReservePaidAmount(item.ReservePayments.ToList(),
                        Reserve.StatusStringType.Guest),
                        item.CouponPrice, item.PrizePrice);
                    var nextItem = reserves.FirstOrDefault(x => x.Id == todayPayments[i + 1].Id);
                    var nextPayablePrice = i < todayPayments.Count - 1 ?
                        PriceUtility.CalculateHostPayablePrice(
                        todayPayments[i + 1].TotalPrice,
                        accounting.GetReservePaidAmount(todayPayments[i + 1].ReservePayments.ToList(),
                            Reserve.StatusStringType.Guest), nextItem.CouponPrice, nextItem.PrizePrice)
                        : 0;
                    currentPrice += hostPayablePrice;
                    reserves.Add(item);
                    if (i >= todayPayments.Count - 1 || currentPrice + nextPayablePrice > maxPrice)
                    {
                        System.IO.Directory.CreateDirectory(Path.Combine(host.WebRootPath, "/content/files/PayList"));
                        var filepath = string.Format("~/content/files/PayList/file{0}{1}", Guid.NewGuid(), ".txt");
                        var fileLines = new List<string>();
                        foreach (var reserve in reserves)
                        {
                            var advertise = reserve.Advertise;
                            var hostUser = users.FirstOrDefault(x => x.Id == advertise.UserID);
                            var bankCard = bankCardService.GetByUserId(advertise.UserID);
                            string bankFullName = string.Empty;
                            if (!string.IsNullOrEmpty(bankCard.FName))
                                bankFullName += bankCard.FName + " ";
                            if (!string.IsNullOrEmpty(bankCard.LName))
                                bankFullName += bankCard.LName;
                            var hostFullName = hostUser.FullName;
                            var price = PriceUtility.CalculateHostPayablePrice(
                                reserve.TotalPrice,
                                accounting.GetReservePaidAmount(reserve.ReservePayments.ToList(),
                                Reserve.StatusStringType.Guest),
                                reserve.CouponPrice, reserve.PrizePrice);
                            fileLines.Add(string.Format("{0},{1},{2},{3}",
                                bankCard.BankCardNumber,
                                price * 10f,
                                !string.IsNullOrEmpty(bankFullName) ? bankFullName : hostFullName,
                                "تسویه کد  رزرو: " + reserve.Id));
                        }
                        using (StreamWriter sw = new StreamWriter(System.IO.File.Open(Path.Combine(host.WebRootPath, filepath), FileMode.Create), Encoding.UTF8))
                        {
                            foreach (var line in fileLines)
                            {
                                sw.WriteLine(line);
                            }
                        }
                        accounting.InsertGroupPayment(new GroupPayment().Init(reserves.Select(x => x.Id), currentPrice, filepath));
                        currentPrice = 0;
                        reserves = new List<Reserve>();
                    }
                }
                return GenerateJsonResult(new
                {
                    status = 1,
                    msg = "فایل های پرداخت گروهی با موفقیت ایجاد شد"
                });
            }
            catch (Exception exc)
            {
                logger.Error("GroupPayment.GenerateGroupPay", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "عملیات با خطای فنی مواجه شد"
                });
            }
        }

        [Auth(UserRoles.Admin)]
        public JsonResult DownloadGroupPaymentFile(int id, bool confirm = false)
        {
            try
            {
                var groupPayment = accounting.FindGroupPayment(id);
                if (groupPayment.Status == GroupPayment.PaymentStatus.Canceled)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "این پرداخت لغو شده است و شما نمیتوانید فایل پرداخت را دانلود کنید"
                    });
                }
                if (groupPayment.Status == GroupPayment.PaymentStatus.Paid)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "این پرداخت قبلا انجام شده است و شما نیازی به دانلود فایل پرداخت ندارید"
                    });
                }
                if (groupPayment.DownloadCount > 0 && !confirm)
                {
                    return GenerateJsonResult(new
                    {
                        status = 2,
                        msg = "این فایل قبلا " + groupPayment.DownloadCount + " بار دریافت شده. آیا میخواهید دوباره دریافت کنید؟"
                    });
                }
                groupPayment.DownloadCount++;
                accounting.UpdateGroupPaymentDownloadCount(groupPayment.Id, groupPayment.DownloadCount);
                return GenerateJsonResult(new
                {
                    status = 1,
                    path = groupPayment.PayListUrl
                });
            }
            catch (Exception exc)
            {
                logger.Error("GroupPayment.DownloadGroupPaymentFile", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "عملیات با خطای فنی مواجه شد"
                });
            }
        }

        [Auth(UserRoles.Admin)]
        public JsonResult CancelPayment(int id)
        {
            try
            {
                var groupPayment = accounting.FindGroupPayment(id);
                if (groupPayment.Status == GroupPayment.PaymentStatus.Canceled)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "این پرداخت قبلا لغو شده است"
                    });
                }
                if (groupPayment.Status == GroupPayment.PaymentStatus.Paid)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "این پرداخت انجام شده است و شما نمیتوانید پرداخت انجام شده را لغو کنید"
                    });
                }
                accounting.UpdateGroupPaymentStatus(groupPayment.Id, GroupPayment.PaymentStatus.Canceled);
                return GenerateJsonResult(new
                {
                    status = 1,
                    msg = "پرداخت گروهی با کد " + id + " با موفقیت لغو شد"
                });
            }
            catch (Exception exc)
            {
                logger.Error("GroupPayment.CancelPayment", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "عملیات با خطای فنی مواجه شد"
                });
            }
        }

        [Auth(UserRoles.Admin)]
        public JsonResult ExcludeFromGroupPayment(int reserveId)
        {
            try
            {
                reserveService.UpdateExcludeGroup(reserveId, true);
                return GenerateJsonResult(new
                {
                    status = 1
                });
            }
            catch (Exception exc)
            {
                logger.Error("GroupPayment.ExcludeFromGroupPayment", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "عملیات با خطای فنی مواجه شد"
                });
            }
        }

        [Auth(UserRoles.Admin)]
        public JsonResult IncludeToGroupPayment(int reserveId)
        {
            try
            {
                reserveService.UpdateExcludeGroup(reserveId, false);
                return GenerateJsonResult(new
                {
                    status = 1
                });
            }
            catch (Exception exc)
            {
                logger.Error("GroupPayment.IncludeToGroupPayment", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "عملیات با خطای فنی مواجه شد"
                });
            }
        }

        [Auth(UserRoles.Admin)]
        public JsonResult ErrorResolved(int reserveId)
        {
            try
            {
                reserveService.UpdatePaymentHasError(reserveId, false);
                return GenerateJsonResult(new
                {
                    status = 1
                });
            }
            catch (Exception exc)
            {
                logger.Error("GroupPayment.ErrorResolved", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "عملیات با خطای فنی مواجه شد"
                });
            }
        }

        [HttpPost]
        public ActionResult UploadBankFile(int id)
        {
            if (Request.Form.Files["FileUpload1"].Length > 0)
            {
                string extension = System.IO.Path.GetExtension(Request.Form.Files["FileUpload1"].FileName).ToLower();

                string[] validFileTypes = { ".xls", ".xlsx", ".csv" };
                System.IO.Directory.CreateDirectory(Path.Combine(host.WebRootPath, "/content/files/PayBankResult"));
                string path1 = string.Format("{0}/{1}", Path.Combine(host.WebRootPath, "/Content/files/PayBankResult"), Request.Form.Files["FileUpload1"].FileName);
                if (validFileTypes.Contains(extension))
                {
                    while (System.IO.File.Exists(path1))
                    {
                        path1 = path1.Replace(extension, "");
                        path1 += "_new" + extension;
                    }
                    using (Stream stream = new FileStream(path1, FileMode.Create))
                    {
                        Request.Form.Files["FileUpload1"].CopyTo(stream);
                    }
                    string msg;
                    var done = ProcessPaymentUpload(id, string.Format("{0}/{1}", "~/Content/files/PayBankResult", Request.Form.Files["FileUpload1"].FileName), out msg);
                    TempData["msg"] = msg;
                    TempData["done"] = done;
                    return RedirectToAction("index");
                }
                else
                {
                    ViewBag.Error = "Please Upload Files in .xlsx format";
                }

            }

            return RedirectToAction("index");
        }

        [Auth(UserRoles.Admin)]
        public ActionResult Download(string path)
        {
            string file = Path.Combine(host.WebRootPath, path);
            string contentType = "text/plain";
            return File(file, contentType, "group_payment_" + DateTime.Now.ToString() + ".txt");
        }

        public bool ProcessPaymentUpload(int id, string path, out string msg)
        {
            try
            {
                var groupPayment = accounting.FindGroupPayment(id);
                if (!string.IsNullOrEmpty(groupPayment.PayResultUrl))
                {
                    msg = "این پرداخت قبلا انجام شده است";
                    return false;
                }
                var reserveIds = groupPayment.GetReserveIds();
                using (SpreadsheetDocument spreadsheetDocument =
                    SpreadsheetDocument.Open(Path.Combine(host.WebRootPath, path), false))
                {
                    WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                    WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                    SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

                    var rows = sheetData.Elements<Row>().ToList();
                    var bankResults = new List<BankResultDTO>();
                    for (int i = 3; i < rows.Count; i++)
                    {
                        bankResults.Add(rows[i].Elements<Cell>().ToList());
                    }

                    var bankResultReserveIds = bankResults.Select(x => x.ReserveId).ToList();
                    var reservePayments = new List<ReservePayment>();
                    var failedReserveIds = reserveIds.Where(x => !bankResultReserveIds.Contains(x));
                    failedReserveIds = failedReserveIds.Concat(bankResults.Where(x => !x.success).Select(x => x.ReserveId));
                    foreach (var bankResult in bankResults)
                    {
                        if (!reserveIds.Contains(bankResult.ReserveId))
                        {
                            msg = "فایل آپلود شده مربوط به این پرداخت نمیباشد";
                            return false;
                        }
                        if (bankResult.success)
                        {
                            var reservePayment = accounting.InsertReservePayment(userAccessor.CurrentUser.Id,
                                bankResult.ReserveId, long.Parse(bankResult.TrackingNumber),
                                long.Parse(bankResult.RefNumber),
                                ReservePayment.ReservePaymentType.SiteClearingToHost,
                                (long)(bankResult.Price / 10f), ReservePayment.ReservePaymentMethod.EPay,
                                userAccessor.CurrentUser.Id, true);
                            if (reservePayment == null)
                            {
                                msg = "پرداخت برای کد رزرو " + bankResult.ReserveId + " کد تراکنش تکراری دارد";
                                return false;
                            }
                            reservePayments.Add(reservePayment);
                        }
                    }
                    accounting.InsertReservePayment(reservePayments);
                    groupPayment.PaymentDone(path, (long)(bankResults.Where(x => x.success).Sum(x => x.Price) / 10f), failedReserveIds.Count());
                    Reserve reserve;
                    BankCard bankCard;
                    string[] bankCardOwnerNameSplit;
                    foreach (var bankResult in bankResults.Where(x => x.success))
                    {
                        reserve = reserveService.Find(bankResult.ReserveId);
                        bankCard = bankCardService.GetByUserId(reserve.Advertise.UserID);
                        bankCardOwnerNameSplit = bankResult.CardOwnerName.Split(' ');
                        bankCard.FName = bankCardOwnerNameSplit[0];
                        string bankCardOwnerLname = "";
                        for (int i = 1; i < bankCardOwnerNameSplit.Length; i++)
                        {
                            if (i == 1)
                            {
                                bankCardOwnerLname = bankCardOwnerNameSplit[i];
                            }
                            else
                            {
                                bankCardOwnerLname += (" " + bankCardOwnerNameSplit[i]);
                            }
                        }
                        bankCard.LName = bankCardOwnerLname;
                        bankCard.LastModifyDate = DateTime.Now;
                        bankCard.BankCardStatus = (int)BankCard.BankCardStatusEnum.Verified;
                        reserveService.UpdatePaymentHasError(failedReserveIds.ToList(), true);
                    }
                    for (int i = 0; i < reservePayments.Count; i++)
                    {
                        var reservePayment = reservePayments[i];
                        var targetReserve = reserveService.Find(reservePayment.ReserveID);
                        var hostUser = userService.Find(targetReserve.Advertise.UserID);

                        accounting.ScheduleSendMessageGroupPayment(new UserContactDTO()
                        {
                            UserEmail = hostUser.Email,
                            UserMainMobile = hostUser.MainMobile,
                            Type = UserContactType.SiteClearingHost,
                            Price = reservePayment.Price.ToString(),
                            AdvertiseId = targetReserve.AdvertiseID.ToString(),
                            TransactionId = reservePayment.TransactionID.ToString(),
                            ReserveId = targetReserve.Id.ToString()
                        }, (i + 1) * 5);
                    }
                }
                msg = "پرداخت ها با موفقیت انجام شدند";
                return true;
            }
            catch (Exception exc)
            {
                msg = "عملیات با خطای فنی مواجه شد";
                return false;
            }
        }

        [Auth(UserRoles.Admin)]
        public ActionResult GetPaymentsListPopup()
        {
            List<Reserve> todayPayments, paymentsWithError, excludingPayments;
            accounting.GetGroupPaymentReserves(out todayPayments, out paymentsWithError, out excludingPayments);

            var model = new Dictionary<GroupPaymentStatusDTO, IEnumerable<GroupPaymentItemDTO>>();
            List<GroupPaymentItemDTO> dtoList = new List<GroupPaymentItemDTO>();

            foreach (var item in todayPayments)
            {
                var hostUser = userService.Find(item.HostUserID);
                var hostBankCard = bankCardService.Find(item.HostUserID);
                var hostBankCardFullName = hostBankCard == null ? "" : hostBankCard.FullName;
                dtoList.Add(new GroupPaymentItemDTO()
                {
                    ReserveId = item.Id,
                    HostUserId = item.HostUserID,
                    GuestUserId = item.UserID,
                    HostBankCardFullName = hostBankCardFullName,
                    HostUserCredit = hostUser.Credit,
                    HostUserFullName = hostUser.FullName,
                    HostPayablePrice = PriceUtility.CalculateHostPayablePrice(item.TotalPrice,
                        accounting.GetReservePaidAmount(item.Id, Reserve.StatusStringType.Guest), item.CouponPrice,
                        item.PrizePrice)
                });
            }
            model.Add(new GroupPaymentStatusDTO(GroupPayment.GroupPaymentStatus.ReadyToPay), dtoList);

            dtoList = new List<GroupPaymentItemDTO>();
            foreach (var item in paymentsWithError)
            {
                var hostUser = userService.Find(item.HostUserID);
                var hostBankCard = bankCardService.Find(item.HostUserID);
                var hostBankCardFullName = hostBankCard == null ? "" : hostBankCard.FullName;
                dtoList.Add(new GroupPaymentItemDTO()
                {
                    ReserveId = item.Id,
                    HostUserId = item.HostUserID,
                    GuestUserId = item.UserID,
                    HostBankCardFullName = hostBankCardFullName,
                    HostUserCredit = hostUser.Credit,
                    HostUserFullName = hostUser.FullName,
                    HostPayablePrice = PriceUtility.CalculateHostPayablePrice(item.TotalPrice,
                        accounting.GetReservePaidAmount(item.Id, Reserve.StatusStringType.Guest), item.CouponPrice,
                        item.PrizePrice)
                });
            }
            model.Add(new GroupPaymentStatusDTO(GroupPayment.GroupPaymentStatus.WithError), dtoList);

            dtoList = new List<GroupPaymentItemDTO>();
            foreach (var item in excludingPayments)
            {
                var hostUser = userService.Find(item.HostUserID);
                var hostBankCard = bankCardService.Find(item.HostUserID);
                var hostBankCardFullName = hostBankCard == null ? "" : hostBankCard.FullName;
                dtoList.Add(new GroupPaymentItemDTO()
                {
                    ReserveId = item.Id,
                    HostUserId = item.HostUserID,
                    GuestUserId = item.UserID,
                    HostBankCardFullName = hostBankCardFullName,
                    HostUserCredit = hostUser.Credit,
                    HostUserFullName = hostUser.FullName,
                    HostPayablePrice = PriceUtility.CalculateHostPayablePrice(item.TotalPrice,
                        accounting.GetReservePaidAmount(item.Id, Reserve.StatusStringType.Guest), item.CouponPrice,
                        item.PrizePrice)
                });
            }
            model.Add(new GroupPaymentStatusDTO(GroupPayment.GroupPaymentStatus.Excluded), dtoList);

            return PartialView("_GroupPaymentList", model);
        }

        [Auth(UserRoles.Admin)]
        public ActionResult GetPaymentDetailPopup(long id)
        {
            GroupPaymentDetailsDTO dto = new GroupPaymentDetailsDTO();
            dto.GroupPayment = accounting.FindGroupPayment((int)id);
            var bankResults = new List<BankResultDTO>();

            if (dto.GroupPayment.Status == GroupPayment.PaymentStatus.Paid &&
                !string.IsNullOrEmpty(dto.GroupPayment.PayResultUrl))
            {
                using (SpreadsheetDocument spreadsheetDocument =
                    SpreadsheetDocument.Open(Path.Combine(host.WebRootPath, dto.GroupPayment.PayResultUrl), false))
                {
                    WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                    WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                    SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

                    var rows = sheetData.Elements<Row>().ToList();
                    for (int i = 3; i < rows.Count; i++)
                    {
                        bankResults.Add(rows[i].Elements<Cell>().ToList());
                    }
                }
            }

            var reserves = reserveService.Find(dto.GroupPayment.GetReserveIds());
            foreach (var reserveItem in reserves)
            {
                var bankCard = bankCardService.GetByUserId(reserveItem.HostUserID);
                var hostBankCardFullName = bankCard == null ? "" : bankCard.FullName;
                var dtoItem = new GroupPaymentItemDTO()
                {
                    ReserveId = reserveItem.Id,
                    HostUserId = reserveItem.HostUserID,
                    HostUserCredit = userService.Find(reserveItem.HostUserID).Credit,
                    HostBankCardFullName = hostBankCardFullName,
                    HostPayablePrice = PriceUtility.CalculateHostPayablePrice(reserveItem.TotalPrice,
                        accounting.GetReservePaidAmount(reserveItem.Id, Reserve.StatusStringType.Guest),
                        reserveItem.CouponPrice, reserveItem.PrizePrice),
                    BankResult = bankResults == null ? null : bankResults.FirstOrDefault(x => x.ReserveId == reserveItem.Id)
                };
                dto.GroupPaymentItems.Add(dtoItem);
            }

            return PartialView("_GroupPaymentDetail", dto);
        }
    }
}
