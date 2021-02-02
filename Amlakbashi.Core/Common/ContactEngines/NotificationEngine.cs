using FirebaseAdmin.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Common.ContactEngines
{
    public static class NotificationEngine
    {
        private static string icon = "/Resource/img/siteicons/icon-192x192.png";
        private static string badge = "/Resource/img/siteicons/badge.png";
        public static void SendMessage(string token, string title, string body, string click_action,
            List<NotificationButton> buttons = null)
        {
            var data = new Dictionary<string, string>()
                {
                    { "title", title },
                    { "body", body },
                    { "icon", icon },
                    { "url" , click_action }
                };
            var actions = new List<FirebaseAdmin.Messaging.Action>();
            if (buttons != null)
            {
                for (int i = 0; i < buttons.Count; i++)
                {
                    var button = buttons[i];
                    var id = "btn" + (i + 1).ToString();
                    data.Add(id, button.Name);
                    data.Add(id + "_title", button.Title);
                    data.Add(button.Name + "_url", button.Url);
                    actions.Add(new FirebaseAdmin.Messaging.Action()
                    {
                        ActionName = button.Name,
                        Title = button.Title
                    });
                }
            }

            // the registration token comes from the client FCM SDKs.
            // See documentation on defining a message payload.
            var message = new Message()
            {
                Token = token,
                Data = data,
                Notification = new Notification()
                {
                    Title = title,
                    Body = body
                },
                Webpush = new WebpushConfig()
                {
                    Data = data,
                    Notification = new WebpushNotification()
                    {
                        Data = data,
                        Title = title,
                        Body = body,
                        Icon = icon,
                        Badge = badge,
                        RequireInteraction = true,
                        CustomData = new Dictionary<string, object>()
                        {
                            { "click_action", click_action }
                        },
                        Actions = actions,
                        Vibrate = new int[] { 200, 100, 200, 100, 200, 100, 200 }
                    }
                }
            };
            // Send a message to the device corresponding to the provided
            // registration token.
            try
            {
                FirebaseMessaging.DefaultInstance.SendAsync(message);
            }
            catch
            {
                // TODO Logger
            }
        }

        public static void SendMessageApplication(string token, string title, string body,
            string target_action = "", string target_id = "0", string channelId = "normal")
        {
            //{ "icon", icon },
            //        { "url", target_action == "" ? "" : target_action + "_" + target_id },
            //        { "channelId", channelId}
            var data = new Dictionary<string, string>()
                {
                    { "title", title },
                    { "body" , body },
                    { "sound", "default" },
                    { "data", target_action == "" ? "" : target_action + "_" + target_id }
                };

            // the registration token comes from the client FCM SDKs.
            // See documentation on defining a message payload.
            var message = new Message()
            {
                Token = token,
                Data = data,
                Android = new AndroidConfig()
                {
                    Notification = new AndroidNotification()
                    {
                        Body = body,
                        Title = title,
                        ChannelId = channelId,
                        Sound = "default",
                    },
                    Data = data,
                    Priority = Priority.High
                },
                Notification = new Notification()
                {
                    Title = title,
                    Body = body
                },
                Webpush = new WebpushConfig()
                {
                    Data = data,
                    Notification = new WebpushNotification()
                    {
                        Title = title,
                        Body = body,
                        Data = data,
                        Vibrate = new int[] { 500, 500, 0, 500, 500, 0, 500, 500 }
                    }
                }
            };
            // Send a message to the device corresponding to the provided
            // registration token.
            try
            {
                FirebaseMessaging.DefaultInstance.SendAsync(message);
            }
            catch
            {
                //LogHelper.LogError(exc);
            }
        }
    }

    public class NotificationButton
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
    }
}
