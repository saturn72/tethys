
using System.Collections.Generic;
using Moq;
using Tethys.Server.Models;
using Tethys.Server.Services.HttpCalls;
using Xunit;
using Shouldly;
using System.Threading.Tasks;
using System;
using Tethys.Server.Services;

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
            var res = await hcSrv.AddHttpCalls(httpCalls);

            res.Status.ShouldBe(ServiceOperationStatus.Fail);
            res.Message.HasValue().ShouldBeTrue();
            hcRepo.Verify(r => r.Create(It.IsAny<IEnumerable<HttpCall>>()), Times.Never());
        }

        [Fact]
        public async Task HttpCallService_AddHttpCall_DeclineOnMissingBucketId()
        {
            var httpCalls = new[]{
                new HttpCall{},
            };

            var hcRepo = new Mock<IHttpCallRepository>();
            var hcSrv = new HttpCallService(null, hcRepo.Object);
            var res = await hcSrv.AddHttpCalls(httpCalls);

            res.Status.ShouldBe(ServiceOperationStatus.Fail);
            res.Message.HasValue().ShouldBeTrue();
            hcRepo.Verify(r => r.Create(It.IsAny<IEnumerable<HttpCall>>()), Times.Never());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task HttpCallService_AddHttpCall_FailToWriteToDatabase(bool partialFailure)
        {
            var cf = partialFailure ? 1 : 2;
            var dbId = 100;
            var bucketPrefix = "some-bucket-id-";
            var startTime = DateTime.UtcNow;
            var httpCalls = new[]{
                new HttpCall{BucketId = bucketPrefix + 1 },
                new HttpCall{BucketId = bucketPrefix + 2},
                new HttpCall{BucketId =bucketPrefix + 3},
                new HttpCall{BucketId = bucketPrefix + 4},
            };

            var hcRepo = new Mock<IHttpCallRepository>();
            hcRepo.Setup(h => h.Create(It.IsAny<IEnumerable<HttpCall>>())).Callback(() =>
            {
                foreach (var h in httpCalls)
                {
                    h.Id = dbId * (cf % 2);
                    cf += cf;
                }
            });

            var hcSrv = new HttpCallService(null, hcRepo.Object);
            var res = await hcSrv.AddHttpCalls(httpCalls);
            res.Status.ShouldBe(partialFailure ? ServiceOperationStatus.Partially : ServiceOperationStatus.Fail);
            res.Message.ShouldContain(partialFailure ? "Some" : "All", Case.Insensitive);
            hcRepo.Verify(r => r.Create(It.IsAny<IEnumerable<HttpCall>>()), Times.Once());
        }

        [Fact]
        public async Task HttpCallService_AddHttpCall_SuccessfulyWritesToDatabase()
        {
            var dbId = 100;
            var bucketPrefix = "some-bucket-id-";
            var startTime = DateTime.UtcNow;
            var httpCalls = new[]{
                new HttpCall{BucketId = bucketPrefix + 1, WasFullyHandled = true, CallsCounter = 100 },
                new HttpCall{BucketId = bucketPrefix + 2, WasFullyHandled = true, CallsCounter = 100},
                new HttpCall{BucketId =bucketPrefix + 3, WasFullyHandled = true, CallsCounter = 100},
                new HttpCall{BucketId = bucketPrefix + 4, WasFullyHandled = true, CallsCounter = 100},
            };

            var hcRepo = new Mock<IHttpCallRepository>();
            hcRepo.Setup(h => h.Create(It.IsAny<IEnumerable<HttpCall>>())).Callback(() =>
            {
                foreach (var h in httpCalls)
                    h.Id = dbId;
            });

            var hcSrv = new HttpCallService(null, hcRepo.Object);
            var res = await hcSrv.AddHttpCalls(httpCalls);
            res.Status.ShouldBe(ServiceOperationStatus.Success);

            hcRepo.Verify(r => r.Create(It.IsAny<IEnumerable<HttpCall>>()), Times.Once());

            foreach (var hc in httpCalls)
            {
                hc.Id.ShouldBe(dbId);
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

        #region GetHttpCalls
        [Fact]
        public async Task HttpCallService_GetHttpCalls()
        {
            var dbId = 100;
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
            var res = await hcSrv.AddHttpCalls(httpCalls);
            res.Status.ShouldBe(ServiceOperationStatus.Success);

            hcRepo.Verify(r => r.Create(It.IsAny<IEnumerable<HttpCall>>()), Times.Once());

            foreach (var hc in httpCalls)
            {
                hc.Id.ShouldBe(dbId);
                hc.BucketId.ShouldStartWith(bucketPrefix);
                hc.BucketId.Length.ShouldBeGreaterThan(bucketPrefix.Length);
                hc.WasFullyHandled.ShouldBeFalse();
                hc.CreatedOnUtc.ShouldBeGreaterThanOrEqualTo(startTime);
                hc.CreatedOnUtc.ShouldBeLessThanOrEqualTo(DateTime.UtcNow);
                hc.AllowedCallsNumber.ShouldBe(1024);
                hc.CallsCounter.ShouldBe(0);
            }
            throw new NotImplementedException("dadada");
        }

        #endregion
    }
}