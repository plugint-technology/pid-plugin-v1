using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using PidPlugin.Extensions;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

namespace PidPlugin.Test.Unit.Extensions
{
    public class HttpClientExtensionsTests
    {
        private const string BaseUrl = "https://fake-url.com/api/";

        private HttpClient Sut { get; set; }
        private Mock<HttpMessageHandler> HttpMessageHandlerMock { get; set; }

        public HttpClientExtensionsTests()
        {
            this.HttpMessageHandlerMock = new Mock<HttpMessageHandler>();

            this.Sut = new HttpClient(this.HttpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }

        public class TheMethod_GetAsync : HttpClientExtensionsTests
        {
            [Fact]
            public async Task Should_call_its_handler_with_proper_url_supplied_getting_response_DTO()
            {
                // arrange

                var expected = new FakeDto()
                {
                    Id          = 2,
                    Description = "Data"
                };

                HttpResponseMessage messageResponse = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(expected))
                };

                this.HttpMessageHandlerMock.Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(messageResponse);

                // act

                var actual = await this.Sut.GetAsync<FakeDto>("data/2065", CancellationToken.None);

                // assert

                actual.Should().BeEquivalentTo(expected);

                var expectedUri = new Uri($"{BaseUrl}data/2065");

                this.HttpMessageHandlerMock.Protected()
                    .Verify("SendAsync",
                        Times.Once(),
                        ItExpr.Is<HttpRequestMessage>(req => req.Content == null && req.Method == HttpMethod.Get && req.RequestUri == expectedUri),
                        ItExpr.IsAny<CancellationToken>()
                    );
            }
        }

        internal class FakeDto
        {
            public int    Id          { get; set; }
            public string Description { get; set; }
        }
    }
}
