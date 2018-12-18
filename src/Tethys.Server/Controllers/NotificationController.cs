using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tethys.Server.Models;
using Tethys.Server.Services;
using Tethys.Server.Services.HttpCalls;
using Tethys.Server.Services.Notifications;

namespace Tethys.Server.Controllers
{
    [Route(Consts.NotificationControllerRoute)]
    public class NotificationController : Controller
    {
        #region CTOR

        public NotificationController(INotificationService notificationeService)
        {
            _notificationeService = notificationeService;
        }

        #endregion

        /// <summary>
        /// Defines array of push notifications to be pushed from server
        /// </summary>
        /// <param name="notifications"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Push([FromBody] IEnumerable<PushNotification> notifications)
        {
            if (notifications == null || !notifications.Any())
                return new ObjectResult("No notifications sent to server")
                {
                    StatusCode = StatusCodes.Status406NotAcceptable
                };

            _notificationeService.NotifyAsync(notifications);

            return Accepted();
        }
        /// <summary>
        /// Stop send notifications.
        /// Note: this command stops push notifications
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult StopAll()
        {
            _notificationeService.Stop();
            return NoContent();
        }

        #region Fields
        private readonly INotificationService _notificationeService;

        #endregion
    }
}