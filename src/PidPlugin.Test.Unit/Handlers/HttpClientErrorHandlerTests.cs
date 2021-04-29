using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using PidPlugin.Dtos;
using PidPlugin.Handlers;
using PidPlugin.Exceptions;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

namespace PidPlugin.Test.Unit.Handlers
{
    public class HttpClientErrorHandlerTests
    {
        public class TheMethod_SendAsync : HttpClientErrorHandlerTests
        {
            [Fact]
            public async Task Should_return_the_response_when_it_was_successful()
            {
                // arrange

                HttpRequestMessage  request  = new HttpRequestMessage();
                HttpResponseMessage expected = new HttpResponseMessage(HttpStatusCode.OK);

                Mock<HttpMessageHandler> httpMessageHandlerMock 
                    = new Mock<HttpMessageHandler>();

                httpMessageHandlerMock
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", request, ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(expected);

                var sut = new HttpClientErrorHandlerAccessor()
                {
                    InnerHandler = httpMessageHandlerMock.Object
                };

                // act

                HttpResponseMessage actual = await sut
                    .SendAsyncAccessor(request, CancellationToken.None);

                // assert

                actual.Should().BeSameAs(expected);
            }

            [Fact]
            public async Task Should_return_pid_plugin_unauthorized_exception_when_response_was_unauthorized()
            {
                // arrange

                HttpRequestMessage  request  = new HttpRequestMessage();
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);

                Mock<HttpMessageHandler> httpMessageHandlerMock 
                    = new Mock<HttpMessageHandler>();

                httpMessageHandlerMock
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", request, ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(response);

                var sut = new HttpClientErrorHandlerAccessor()
                {
                    InnerHandler = httpMessageHandlerMock.Object
                };

                // act

                await Assert.ThrowsAsync<PidPluginUnauthorizedException>(() => sut
                    .SendAsyncAccessor(request, CancellationToken.None));
            }

            [Fact]
            public async Task Should_return_pid_plugin_validation_exception_with_its_message_when_response_was_bad_request()
            {
                // arrange

                string messageExpected = "error message description";

                ErrorResponse errorResponse = new ErrorResponse()
                {
                    Message = messageExpected
                };

                HttpRequestMessage  request  = new HttpRequestMessage();
                
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(errorResponse))
                };

                Mock<HttpMessageHandler> httpMessageHandlerMock 
                    = new Mock<HttpMessageHandler>();

                httpMessageHandlerMock
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", request, ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(response);

                var sut = new HttpClientErrorHandlerAccessor()
                {
                    InnerHandler = httpMessageHandlerMock.Object
                };

                // act

                var actual = await Assert.ThrowsAsync<PidPluginValidationException>(() => sut
                    .SendAsyncAccessor(request, CancellationToken.None));

                actual.Message.Should().BeEquivalentTo(messageExpected);
            }

            [Fact]
            public async Task Should_return_pid_plugin_exception_when_response_was_not_successfull_with_unhandled_http_code()
            {
                // arrange

                HttpRequestMessage  request  = new HttpRequestMessage();
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError);

                Mock<HttpMessageHandler> httpMessageHandlerMock
                    = new Mock<HttpMessageHandler>();

                httpMessageHandlerMock
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", request, ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(response);

                var sut = new HttpClientErrorHandlerAccessor()
                {
                    InnerHandler = httpMessageHandlerMock.Object
                };

                // act

                var actual = await Assert.ThrowsAsync<PidPluginException>(() => sut
                    .SendAsyncAccessor(request, CancellationToken.None));

                actual.Message.Should().BeEquivalentTo(response.ReasonPhrase);
            }
        }

        internal class HttpClientErrorHandlerAccessor : HttpClientErrorHandler
        {
            public async Task<HttpResponseMessage> SendAsyncAccessor(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return await this.SendAsync(request, cancellationToken);
            }
        }
    }
}
