using FirebaseAdmin.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Common.ContactEngines
{
    public static class AppNotificationEngine
    {
        //private static string icon = "/Resource/img/siteicons/icon-192x192.png";
        //private static string badge = "/Resource/img/siteicons/badge.png";
        public static void SendMessage(string token, string title, string body,
            string channelId = "normal", string target_action = "", string target_id = "0")
        {
            var data = new Dictionary<string, string>()
                {
                    { "experienceId", "@rasoul_sh/Amlakbashi-react-native" },
                    { "title", title },
                    { "message" , body },
                    { "body", target_action == "" ? "" : target_action + "_" + target_id },
                    { "channelId", channelId}
                };

            // the registration token comes from the client FCM SDKs.
            // See documentation on defining a message payload.
            var message = new Message()
            {
                Token = token,
                Android = new AndroidConfig()
                {
                    Priority = Priority.High,
                    Data = data,
                }
            };
            // Send a message to the device corresponding to the provided
            // registration token.
            try
            {
                FirebaseMessaging.DefaultInstance.SendAsync(message);
            }
            catch (Exception exc)
            {
                // TODO logger
            }
        }
    }
}
