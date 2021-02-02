using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using static Amlakbashi.Core.Entities.Chat;

namespace Amlakbashi.Core.DTOs.ReserveChatDTOs
{
    [Serializable]
    public class ReserveChatItemDTO
    {
        public List<ChatWordDTO> words;
        public bool hasForbiddenCharacter;
        public bool isOwnText;
        public ReadStatusEnum readStatus;
        public string createTimeString;

        public static ReserveChatItemDTO Generate(Chat chat, int userId,
            IQueryable<Advertise> allAdvertises)
        {
            var result = new ReserveChatItemDTO();
            string text;
            result.hasForbiddenCharacter = ChatLocalization.
                TextHasForbiddenCharacters(chat.Text, out text, allAdvertises);
            var rawWords = text.Split(' ');
            result.words = new List<ChatWordDTO>();
            foreach (var rawWord in rawWords)
            {
                var word = new ChatWordDTO();
                word.word = rawWord;
                long advertiseId;
                if (long.TryParse(rawWord, out advertiseId))
                {
                    var acc = allAdvertises.FirstOrDefault(x => x.Id == advertiseId);
                    if (acc != null)
                    {
                        word.isAdvertiseId = true;
                        word.advertiseSlug = acc.Slug;
                    }
                }
                result.words.Add(word);
            }

            result.isOwnText = chat.UserID == userId;
            result.readStatus = (ReadStatusEnum)chat.ReadStatus;
            result.createTimeString = DateTimeUtility.GregorianToPersianDate(
                chat.CreateTime).Replace(',', '/') + " " + chat.CreateTime.ToString("HH:mm");
            return result;
        }

        [Serializable]
        public class ChatWordDTO
        {
            public string word;
            public bool isAdvertiseId;
            public string advertiseSlug;
        }
    }
}
