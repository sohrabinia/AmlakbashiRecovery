using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Responses.SupportChats
{
    public class SupportChatResponse
    {
        public List<SupportChatMessageResponse> messages { get; set; } = new List<SupportChatMessageResponse>();
    }

    public class SupportChatMessageResponse
    {
        public string message { get; set; }
        public string time { get; set; }
        public bool forUser { get; set; }
        public bool viewed { get; set; }
    }
}
