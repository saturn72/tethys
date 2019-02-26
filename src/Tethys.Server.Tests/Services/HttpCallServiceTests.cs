using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Shouldly;
using Tethys.Server.DbModel;
using Tethys.Server.Models;
using Tethys.Server.Services.HttpCalls;
using Xunit;

namespace Tethys.Server.Tests.Services
{
    public class HttpCallServiceTests
    {
        [Theory]
        [MemberData(nameof(GetNextHttpCall_ReturnNullOnEmptyResultFromRepository_DATA))]
        public async Task GetNextHttpCall_ReturnNullOnEmptyResultFromRepositoryAsync(IEnumerable<HttpCall> emptyResult)
        {
            var hcRepo = new Mock<IRepository<HttpCall>>();
            hcRepo.Setup(h => h.GetAll(It.IsAny<Func<HttpCall, bool>>())).Returns(emptyResult);

            var srv = new HttpCallService(null, hcRepo.Object);
            var res = await srv.GetNextHttpCall(null);
            res.ShouldBeNull();
        }

        public static IEnumerable<object[]> GetNextHttpCall_ReturnNullOnEmptyResultFromRepository_DATA => new[]{
            new []{null as IEnumerable<object>},
            new []{new HttpCall[]{}},
            };

        [Fact]
        public async Task GetNextHttpCall_ReturnCollectionElement()
        {
            var expDate = DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(10));
            var hcRepo = new Mock<IRepository<HttpCall>>();
            var expCollection = new[]{
                new HttpCall{Id = 1},
                new HttpCall{Id = 2},
                new HttpCall{Id = 3},
                new HttpCall{
                    Id = 4,
                    CallsCounter = 1,
                    AllowedCallsNumber = 2,
                    HandledOnUtc = expDate
                }
            };
            hcRepo.Setup(h => h.GetAll(It.IsAny<Func<HttpCall, bool>>())).Returns(expCollection);

            var srv = new HttpCallService(null, hcRepo.Object);
            var res = await srv.GetNextHttpCall(null);
            res.Id.ShouldBe(4);
            res.WasFullyHandled.ShouldBeTrue();
            res.HandledOnUtc.Value.ShouldBeGreaterThan(expDate);
            res.CallsCounter.ShouldBe(res.AllowedCallsNumber);

            hcRepo.Verify(h => h.Update(It.Is<HttpCall>(hc => hc == expCollection[3])), Times.Once);
        }

    }
}