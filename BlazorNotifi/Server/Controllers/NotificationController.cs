using BlazorNotifi.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebPush;

namespace BlazorNotifi.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationController : ControllerBase
    {
        private static readonly List<NotificationSubscription> Subscriptions = new();

        [HttpPost]
        [Route("subscribe")]
        public int Post(NotificationSubscription notificationSubscription)
        {
            Subscriptions.Add(notificationSubscription);
            return Subscriptions.Count();
        }

        [HttpGet]
        [Route("sendall")]
        public async Task<int> Get()
        {
            //Replace with your generated public/private key
            const string publicKey = "BDX8x7dX4jaALPG94e0vSYnbbnlRL4_ktiANFe9-FzyP2JzjRVGqQR33NXSk5oFLi8qLyYT_Dx4mM_KfsrkvG-8";
            const string privateKey = "P1MgiIQxxVp-R4FWkQZRBKAWcH9VVxVPcr0gVcMCv8g";

            //give a website URL or mailto:your-mail-id
            var vapidDetails = new VapidDetails("http://mkumaran.net", publicKey, privateKey);
            var webPushClient = new WebPushClient();

            foreach (var subscription in Subscriptions)
            {
                var pushSubscription = new PushSubscription(subscription.Url, subscription.P256dh, subscription.Auth);

                try
                {
                    var payload = JsonSerializer.Serialize(new
                    {
                        message = "this text is from server",
                        url = "open this URL when user clicks on notification"
                    });
                    await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
                }
                catch (Exception ex)
                {
                    await Console.Error.WriteLineAsync("Error sending push notification: " + ex.Message);
                    //return -1;
                }
            }

            return Subscriptions.Count();
        }
    }
}
