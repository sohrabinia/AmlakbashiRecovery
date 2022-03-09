using Entities = Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Responses.User
{
    public class LoginResponse
    {
        public string guid { get; set; }
        public string username { get; set; }
        public Entities.User.UserState state { get; set; }
        public bool isNewUser { get; set; } = false;
        public bool isIranNumber { get; set; } = true;
        public string stateDesc
        {
            get
            {
                return state.ToString();
            }
        }
    }
}
