using Amlakbashi.Core.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Requests.Reserves
{
    public class ReservePostCancelRequest
    {
        [Range(1, int.MaxValue)]
        public int reserveId { get; set; }

        [Range(0, 200)]
        public Reserve.ReserveCancelReasons reason { get; set; }

        public string reasonDesc { get; set; }

        [BindNever]
        public int userId { get; set; }

        [BindNever]
        public Entities.User.UserGeneralTypeEnum panel { get; set; }

        [BindNever]
        public ActionLog.ActionSourceEnum actionSource { get; set; }

        [BindNever]
        public Reserve.ReserveCancelType cancelType { 
            get 
            {
                if (reason > 0 && (int)reason <= 50)
                {
                    return Reserve.ReserveCancelType.CancelByGuestForGuestProblem;
                }
                else if ((int)reason > 50 && (int)reason <= 100)
                {
                    return Reserve.ReserveCancelType.CancelByGuestForHostProblem;
                }
                else if ((int)reason > 100 && (int)reason <= 150)
                {
                    return Reserve.ReserveCancelType.CancelByHostForHostProblem;
                }
                else if ((int)reason > 150 && (int)reason <= 200)
                {
                    return Reserve.ReserveCancelType.CancelByHostForGuestProblem;
                }
                else
                {
                    return Reserve.ReserveCancelType.Unset;
                }
            } 
        }
    }
}
