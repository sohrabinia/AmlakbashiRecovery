using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Amlakbashi.Core.DTOs.ReserveChatDTOs
{
    [Serializable]
    public class ReserveChatPopupDTO
    {
        public bool isGuest;
        public List<ReserveChatItemDTO> items;

        public static ReserveChatPopupDTO Generate(List<Chat> chats,
            int userId, int guestUserId, IQueryable<Advertise> allAdvertises)
        {
            var result = new ReserveChatPopupDTO();
            result.isGuest = guestUserId == userId;
            result.items = new List<ReserveChatItemDTO>();
            chats.ForEach(chat => result.items.Add(ReserveChatItemDTO.Generate(
                chat, userId, allAdvertises)));
            return result;
        }
    }
}
