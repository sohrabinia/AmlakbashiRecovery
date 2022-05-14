using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Requests.User
{
    public class ChangePanelRequest
    {
        [Range(0, 1)]
        public Entities.User.UserGeneralTypeEnum panel { get; set; }
    }
}
