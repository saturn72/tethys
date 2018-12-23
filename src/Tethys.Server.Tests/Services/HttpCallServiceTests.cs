
using System.Collections.Generic;
using Moq;
using Tethys.Server.Models;
using Tethys.Server.Services.HttpCalls;
using Xunit;
using Shouldly;
using System.Threading.Tasks;
using System;

namespace Tethys.Server.Tests.Services
{
    public class HttpCallServiceTests
    {

        #region Reset
        [Fact]
        public void HttpCallService_Reset()
        {
            var hcRepo = new Mock<IHttpCallRepository>();
            var hcSrv = new HttpCallService(null, hcRepo.Object);
            hcSrv.Reset();
            hcRepo.Verify(r => r.FlushUnhandled(), Times.Once());
        }
        #endregion

        #region Create

        public static IEnumerable<object[]> EmptyHttpCallCollection =>
        new[]{
            new object[]{null},
            new object[]{new HttpCall[]{}}
        };

        [Theory]
        [MemberData(nameof(EmptyHttpCallCollection))]
        public async Task HttpCallService_AddHttpCall_OnEmptyCollectionAsync(IEnumerable<HttpCall> httpCalls)
        {
            var hcRepo = new Mock<IHttpCallRepository>();
            var hcSrv = new HttpCallService(null, hcRepo.Object);
            await hcSrv.AddHttpCalls(httpCalls);
            hcRepo.Verify(r => r.Create(It.IsAny<IEnumerable<HttpCall>>()), Times.Never());
        }

        [Fact]
        public async Task HttpCallService_AddHttpCall_WritesToDatabse()
        {
            var bucketPrefix = "some-bucket-id-";
            var startTime = DateTime.UtcNow;
            var httpCalls = new[]{
                new HttpCall{BucketId = bucketPrefix + 1, WasFullyHandled = true, CallsCounter = 100 },
                new HttpCall{BucketId = bucketPrefix + 2, WasFullyHandled = true, CallsCounter = 100},
                new HttpCall{BucketId =bucketPrefix + 3, WasFullyHandled = true, CallsCounter = 100},
                new HttpCall{BucketId = bucketPrefix + 4, WasFullyHandled = true, CallsCounter = 100},
            };

            var hcRepo = new Mock<IHttpCallRepository>();
            var hcSrv = new HttpCallService(null, hcRepo.Object);
            await hcSrv.AddHttpCalls(httpCalls);
            hcRepo.Verify(r => r.Create(It.IsAny<IEnumerable<HttpCall>>()), Times.Once());

            foreach (var hc in httpCalls)
            {
                hc.BucketId.ShouldStartWith(bucketPrefix);
                hc.BucketId.Length.ShouldBeGreaterThan(bucketPrefix.Length);
                hc.WasFullyHandled.ShouldBeFalse();
                hc.CreatedOnUtc.ShouldBeGreaterThanOrEqualTo(startTime);
                hc.CreatedOnUtc.ShouldBeLessThanOrEqualTo(DateTime.UtcNow);
                hc.AllowedCallsNumber.ShouldBe(1024);
                hc.CallsCounter.ShouldBe(0);
            }
        }


        #endregion
    }
}