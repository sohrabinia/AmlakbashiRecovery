using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Requests.SupportChats
{
    public class SupportChatPostMessageRequest
    {
        public int id { get; set; }
        public string message { get; set; }
    }
}
