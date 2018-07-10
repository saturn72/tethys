using System;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Xunit;
using Tethys.Server.IntegrationTests.Components;
using System.Collections.Generic;

namespace Tethys.Server.IntegrationTests
{
    public abstract class IntegrationTestBase : IClassFixture<TethysServerWebApplicationFactory>
    , IDisposable
    {
        private readonly CancellationTokenSource _signalRCancelationSource;
        private readonly HubConnection _connection;
        protected readonly HttpClient Client;
        protected readonly ICollection<RecievedNotification> RecievedNotifications;
        public IntegrationTestBase()
        {
            var factory = new TethysServerWebApplicationFactory();
            Client = factory.CreateClient();
            // Client = new HttpClient()
            // {
            //     BaseAddress = new Uri(BaseAddress),
            // };

            //        factory.CreateClient();
            RecievedNotifications = new List<RecievedNotification>();
            _signalRCancelationSource = new CancellationTokenSource();
            _connection = InitSignalR(factory.Server.BaseAddress).Result;
        }
        public async Task<HubConnection> InitSignalR(Uri baseAddress)
        {
            var wsUri = new Uri(baseAddress, "ws");
            var connection = new HubConnectionBuilder()
                  .WithUrl(wsUri)
                  .Build();

            connection.On<string, string>(Consts.PushNotificationLog, (key, body) => RecievedNotifications.Add(new RecievedNotification
            {
                RecievedOnUtc = DateTime.UtcNow,
                Key = key,
                Body = body
            }));
            await connection.StartAsync(_signalRCancelationSource.Token);
            return connection;
        }
        #region Utilities

        protected HttpRequestMessage BuildHttpRequestMessage(object notifications, string uri)
        {
            var json = JsonConvert.SerializeObject(notifications, Formatting.None); ;
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var req = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = content,
            };

            return req;
        }

        #endregion

        #region IDisposable

        private bool _disposing;
        public void Dispose()
        {
            if (_disposing)
                return;
            _disposing = true;
            _signalRCancelationSource.Cancel();
            _disposing = false;
        }
        #endregion
    }
}