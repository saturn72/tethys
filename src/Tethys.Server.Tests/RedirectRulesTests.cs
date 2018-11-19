using System;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Shouldly;
using Tethys.Server.Models;

namespace Tethys.Server.Tests
{
    public class RedirectRulesTests
    {

        [Theory]
        [InlineData(Consts.ApiBaseUrl)]
        [InlineData(Consts.SwaggerEndPointPrefix)]
        [InlineData(Consts.TethysWebSocketPath)]
        public void NotInterceptedRequest(string path)
        {
            var req = new Mock<HttpRequest>();
            req.Setup(hr => hr.Path).Returns(path);
            RedirectRules.RedirectRequests(req.Object, null);

            req.Verify(h => h.HttpContext.Items, Times.Never);
        }

        [Fact]
        public void InterceptHttpRequest_HttpRequest()
        {
            const string p = "/path",
            qs = "?queryString",
            method = "method";

            var httpContextItems = new Dictionary<object, object>();
            var hc = new Mock<HttpContext>();
            hc.Setup(h => h.Items).Returns(httpContextItems);
            var req = new Mock<HttpRequest>();
            req.Setup(hr => hr.Path).Returns(p);
            req.Setup(hr => hr.QueryString).Returns(new QueryString(qs));
            req.Setup(hr => hr.Method).Returns(method);
            req.Setup(hr => hr.HttpContext).Returns(hc.Object);

            var tc = new TethysConfig
            {
                WebSocketSuffix = new[] { "sss" }
            };
            RedirectRules.RedirectRequests(req.Object, tc);

            httpContextItems.Keys.Count.ShouldBe(1);
            var or = httpContextItems[Consts.OriginalRequest].ShouldBeOfType<Request>();
            or.Resource.ShouldBe(p);
            or.Query.ShouldBe(qs);
            or.HttpMethod.ShouldBe(method);

            req.VerifySet(h => h.Path = Consts.MockControllerRoute, Times.Once);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void InterceptHttpRequest_WebSocketRequest(bool isNegotiation)
        {
            const string p = "/wss",
            qs = "?queryString",
            method = "method";

            var returnPath = isNegotiation ? p + Consts.TethysWebSocketPathNegotiate : p;

            var httpContextItems = new Dictionary<object, object>();
            var hc = new Mock<HttpContext>();
            hc.Setup(h => h.Items).Returns(httpContextItems);
            var req = new Mock<HttpRequest>();
            req.Setup(hr => hr.Path).Returns(returnPath);
            req.Setup(hr => hr.QueryString).Returns(new QueryString(qs));
            req.Setup(hr => hr.Method).Returns(method);
            req.Setup(hr => hr.HttpContext).Returns(hc.Object);

            var tc = new TethysConfig
            {
                WebSocketSuffix = new[] { p }
            };
            RedirectRules.RedirectRequests(req.Object, tc);

            httpContextItems.Keys.Count.ShouldBe(1);
            var or = httpContextItems[Consts.OriginalRequest].ShouldBeOfType<Request>();
            or.Query.ShouldBe(qs);
            or.HttpMethod.ShouldBe(method);

            var expPath = isNegotiation
            ? Consts.TethysWebSocketPath + Consts.TethysWebSocketPathNegotiate
            : Consts.TethysWebSocketPath;
            req.VerifySet(h => h.Path = expPath, Times.Once);
        }
    }
}