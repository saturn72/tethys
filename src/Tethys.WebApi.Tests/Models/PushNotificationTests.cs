using Shouldly;
using Tethys.WebApi.Models;
using Xunit;

namespace Tethys.WebApi.Tests.Models
{
    public class PushNotificationTests
    {
        [Fact]
        public void PushNotification_WasFullyHandles(){
            var pn = new PushNotification();
            pn.WasFullyHandled.ShouldBeTrue();
            pn.NotifyTimes = 1;
            pn.WasFullyHandled.ShouldBeFalse();
        }        
    }
}