using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Requests.Chats
{
    public class ChatPostMessageRequest
    {
        public long reserveId { get; set; }
        public string message { get; set; }
    }
}
