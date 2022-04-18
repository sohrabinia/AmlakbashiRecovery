using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Responses.Chats
{
    public class ChatResponse
    {
        public string message { get; set; }
        public string time { get; set; }
        public bool forUser { get; set; }
        public bool viewed { get; set; }
    }
}
