using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using PidPlugin.Cache;
using PidPlugin.Dtos;
using FluentAssertions;
using Moq;
using Microsoft.Extensions.Options;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

namespace PidPlugin.Test.Unit
{
    public class PidPluginClientTests
    {
        private string BaseUrl => "http://pid-plugin-service.com/api/";

        private PidPluginClient Sut { get; set; }

        private Mock<HttpMessageHandler>        HttpMessageHandlerMock  { get; set; }
        private HttpClient                      HttpClient              { get; set; }
        private Mock<ICacheAccessor>            CacheAccessorMock       { get; set; }
        private IOptions<CacheOptions>          CacheOptions            { get; set; }

        public PidPluginClientTests()
        {
            this.CacheAccessorMock      = new Mock<ICacheAccessor>();
            this.HttpMessageHandlerMock = new Mock<HttpMessageHandler>();

            this.CacheOptions = Options.Create(new CacheOptions()
            {
                BankAccountDetailExpiration    = TimeSpan.FromSeconds(1),
                EntityDataBasicExpiration      = TimeSpan.FromSeconds(1),
                EntityDataFullExpiration       = TimeSpan.FromSeconds(1),
                BankAccountOwnershipExpiration = TimeSpan.FromSeconds(1),
                SpecialRecordsExpiration       = TimeSpan.FromSeconds(1),
            });

            this.HttpClient = new HttpClient(this.HttpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri(this.BaseUrl)
            };

            this.Sut = new PidPluginClient(this.HttpClient, this.CacheAccessorMock.Object, this.CacheOptions);
        }

        public class The_Constructor : PidPluginClientTests
        {
            [Fact]
            public void Should_throw_argument_null_exception_when_http_client_is_null()
            {
                // arrange

                HttpClient httpClient = null;

                // act & assert

                Assert.Throws<ArgumentNullException>(nameof(httpClient), 
                    () => new PidPluginClient(httpClient, this.CacheAccessorMock.Object, this.CacheOptions));
            }

            [Fact]
            public void Should_throw_argument_null_exception_when_cache_accessor_is_null()
            {
                // arrange

                ICacheAccessor cacheAccessor = null;

                // act & assert

                Assert.Throws<ArgumentNullException>(nameof(cacheAccessor),
                    () => new PidPluginClient(this.HttpClient, cacheAccessor, this.CacheOptions));
            }

            [Fact]
            public void Should_throw_argument_null_exception_when_options_is_null()
            {
                // arrange

                IOptions<CacheOptions> options = null;

                // act & assert

                Assert.Throws<ArgumentNullException>(nameof(options),
                    () => new PidPluginClient(this.HttpClient, this.CacheAccessorMock.Object, options));
            }
        }

        public class The_Method_GetEntityDataBasicAsync : PidPluginClientTests
        {
            [Fact]
            public async Task Should_call_http_client_with_correct_url_if_response_is_not_cached()
            {
                // arrange

                string            cuit              = "5050505050";
                CancellationToken cancellationToken = CancellationToken.None;

                EntityBasicData expected = new EntityBasicData()
                {
                    NaturalKey = 56
                };

                string url = $"EntityDataBasic?key={cuit}";

                Uri expectedUri = new Uri($"{this.HttpClient.BaseAddress}{url}");

                this.HttpMessageHandlerMock
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(expected))
                    });

                // act

                var actual = await this.Sut.GetEntityDataBasicAsync(cuit, cancellationToken);

                // assert

                actual.Should().BeEquivalentTo(expected);

                this.HttpMessageHandlerMock
                    .Protected()
                    .Verify(
                        "SendAsync",
                        Times.Once(),
                        ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUri),
                        ItExpr.IsAny<CancellationToken>()
                    );
            }

            [Fact]
            public async Task Should_not_call_http_client_if_response_is_cached()
            {
                // arrange

                string            cuit              = "5050505050";
                CancellationToken cancellationToken = CancellationToken.None;

                EntityBasicData expected = new EntityBasicData()
                {
                    NaturalKey = 56
                };

                string url = $"EntityDataBasic?key={cuit}";
                
                object value = expected;

                this.CacheAccessorMock
                    .Setup(x => x.TryGetValue(It.Is<string>(u => u.ToString() == url), out value))
                    .Returns(true);

                // act

                var actual = await this.Sut.GetEntityDataBasicAsync(cuit, cancellationToken);

                // assert

                actual.Should().BeEquivalentTo(expected);

                this.HttpMessageHandlerMock
                    .Protected()
                    .Verify(
                        "SendAsync",
                        Times.Never(),
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>()
                    );

                this.CacheAccessorMock.Verify(
                    x => x.TryGetValue(
                        It.IsAny<string>(),
                        out value
                    ),
                    Times.Once()
                );
            }

            [Fact]
            public async Task Should_cache_response_when_response_is_successful()
            {
                // arrange

                string            cuit              = "5050505050";
                CancellationToken cancellationToken = CancellationToken.None;

                EntityBasicData expected = new EntityBasicData()
                {
                    NaturalKey = 56
                };

                string url = $"EntityDataBasic?key={cuit}";
                
                object value = null;

                this.CacheAccessorMock
                    .Setup(x => x.TryGetValue(It.Is<string>(u => u.ToString() == url), out value))
                    .Returns(false);

                this.HttpMessageHandlerMock
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(expected))
                    });

                // act

                var actual = await this.Sut.GetEntityDataBasicAsync(cuit, cancellationToken);

                // assert

                actual.Should().BeEquivalentTo(expected);

                this.HttpMessageHandlerMock
                    .Protected()
                    .Verify(
                        "SendAsync",
                        Times.Once(),
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>()
                    );

                this.CacheAccessorMock.Verify(
                    x => x.Set(
                        It.Is<string>(p => p == url),
                        It.IsAny<object>(),
                        this.CacheOptions.Value.EntityDataBasicExpiration
                    ),
                    Times.Once()
                );
            }
        }

        public class The_Method_GetEntityDataFullAsync : PidPluginClientTests
        {
            [Fact]
            public async Task Should_call_http_client_with_correct_url_if_response_is_not_cached()
            {
                // arrange

                string            cuit              = "5050505050";
                CancellationToken cancellationToken = CancellationToken.None;

                EntityBasicData expected = new EntityBasicData()
                {
                    NaturalKey = 56
                };

                string url = $"EntityDataFull?key={cuit}";

                Uri expectedUri = new Uri($"{this.HttpClient.BaseAddress}{url}");

                this.HttpMessageHandlerMock
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(expected))
                    });

                // act

                var actual = await this.Sut.GetEntityDataFullAsync(cuit, cancellationToken);

                // assert

                actual.Should().BeEquivalentTo(expected);

                this.HttpMessageHandlerMock
                    .Protected()
                    .Verify(
                        "SendAsync",
                        Times.Once(),
                        ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUri),
                        ItExpr.IsAny<CancellationToken>()
                    );
            }

            [Fact]
            public async Task Should_not_call_http_client_if_response_is_cached()
            {
                // arrange

                string            cuit              = "5050505050";
                CancellationToken cancellationToken = CancellationToken.None;

                EntityFullData expected = new EntityFullData()
                {
                    NaturalKey = 56
                };

                string url = $"EntityDataFull?key={cuit}";

                object value = expected;

                this.CacheAccessorMock
                    .Setup(x => x.TryGetValue(It.Is<string>(u => u.ToString() == url), out value))
                    .Returns(true);

                // act

                var actual = await this.Sut.GetEntityDataFullAsync(cuit, cancellationToken);

                // assert

                actual.Should().BeEquivalentTo(expected);

                this.HttpMessageHandlerMock
                    .Protected()
                    .Verify(
                        "SendAsync",
                        Times.Never(),
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>()
                    );

                this.CacheAccessorMock.Verify(
                    x => x.TryGetValue(
                        It.IsAny<string>(),
                        out value
                    ),
                    Times.Once()
                );
            }

            [Fact]
            public async Task Should_cache_response_when_response_is_successful()
            {
                // arrange

                string            cuit              = "5050505050";
                CancellationToken cancellationToken = CancellationToken.None;

                EntityBasicData expected = new EntityBasicData()
                {
                    NaturalKey = 56
                };

                string url = $"EntityDataFull?key={cuit}";

                object value = null;

                this.CacheAccessorMock
                    .Setup(x => x.TryGetValue(It.Is<string>(u => u.ToString() == url), out value))
                    .Returns(false);

                this.HttpMessageHandlerMock
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(expected))
                    });

                // act

                var actual = await this.Sut.GetEntityDataFullAsync(cuit, cancellationToken);

                // assert

                actual.Should().BeEquivalentTo(expected);

                this.HttpMessageHandlerMock
                    .Protected()
                    .Verify(
                        "SendAsync",
                        Times.Once(),
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>()
                    );

                this.CacheAccessorMock.Verify(
                    x => x.Set(
                        It.Is<string>(p => p == url),
                        It.IsAny<object>(),
                        this.CacheOptions.Value.EntityDataBasicExpiration
                    ),
                    Times.Once()
                );
            }
        }

        public class The_Method_GetBankAccountDetailAsync : PidPluginClientTests
        {
            [Fact]
            public async Task Should_call_http_client_with_correct_url_if_response_is_not_cached()
            {
                // arrange

                string            cbu               = "2323232323";
                CancellationToken cancellationToken = CancellationToken.None;

                BankAccountDetail expected = new BankAccountDetail()
                {
                    currency = "pesos"
                };

                string url = $"BankAccountDetails?key={cbu}";

                Uri expectedUri = new Uri($"{this.HttpClient.BaseAddress}{url}");

                this.HttpMessageHandlerMock
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(expected))
                    });

                // act

                var actual = await this.Sut.GetBankAccountDetailAsync(cbu, cancellationToken);

                // assert

                actual.Should().BeEquivalentTo(expected);

                this.HttpMessageHandlerMock
                    .Protected()
                    .Verify(
                        "SendAsync",
                        Times.Once(),
                        ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUri),
                        ItExpr.IsAny<CancellationToken>()
                    );
            }

            [Fact]
            public async Task Should_not_call_http_client_if_response_is_cached()
            {
                // arrange

                string            cbu               = "23232323";
                CancellationToken cancellationToken = CancellationToken.None;

                BankAccountDetail expected = new BankAccountDetail()
                {
                    currency = "pesos"
                };

                string url = $"BankAccountDetails?key={cbu}";

                object value = expected;

                this.CacheAccessorMock
                    .Setup(x => x.TryGetValue(It.Is<string>(u => u.ToString() == url), out value))
                    .Returns(true);

                // act

                var actual = await this.Sut.GetBankAccountDetailAsync(cbu, cancellationToken);

                // assert

                actual.Should().BeEquivalentTo(expected);

                this.HttpMessageHandlerMock
                    .Protected()
                    .Verify(
                        "SendAsync",
                        Times.Never(),
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>()
                    );

                this.CacheAccessorMock.Verify(
                    x => x.TryGetValue(
                        It.IsAny<string>(),
                        out value
                    ),
                    Times.Once()
                );
            }

            [Fact]
            public async Task Should_cache_response_when_response_is_successful()
            {
                // arrange

                string            cbu               = "2323232323";
                CancellationToken cancellationToken = CancellationToken.None;

                BankAccountDetail expected = new BankAccountDetail()
                {
                    currency = "pesos"
                };

                string url = $"BankAccountDetails?key={cbu}";

                object value = null;

                this.CacheAccessorMock
                    .Setup(x => x.TryGetValue(It.Is<string>(u => u.ToString() == url), out value))
                    .Returns(false);

                this.HttpMessageHandlerMock
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(expected))
                    });

                // act

                var actual = await this.Sut.GetBankAccountDetailAsync(cbu, cancellationToken);

                // assert

                actual.Should().BeEquivalentTo(expected);

                this.HttpMessageHandlerMock
                    .Protected()
                    .Verify(
                        "SendAsync",
                        Times.Once(),
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>()
                    );

                this.CacheAccessorMock.Verify(
                    x => x.Set(
                        It.Is<string>(p => p == url),
                        It.IsAny<object>(),
                        this.CacheOptions.Value.EntityDataBasicExpiration
                    ),
                    Times.Once()
                );
            }
        }

        public class The_Method_GetBankAccountOwnershipAsync : PidPluginClientTests
        {
            [Fact]
            public async Task Should_call_http_client_with_correct_url_if_response_is_not_cached()
            {
                // arrange

                string            cuit              = "5050505050";
                string            cbu               = "2323232323";
                CancellationToken cancellationToken = CancellationToken.None;

                BankAccountOwner expected = new BankAccountOwner()
                {
                    valid = true
                };

                string url = $"BankAccountOwnership?account_address={cbu}&owner_key={cuit}";

                Uri expectedUri = new Uri($"{this.HttpClient.BaseAddress}{url}");

                this.HttpMessageHandlerMock
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(expected))
                    });

                // act

                var actual = await this.Sut.GetBankAccountOwnershipAsync(cbu, cuit, cancellationToken);

                // assert

                actual.Should().BeEquivalentTo(expected);

                this.HttpMessageHandlerMock
                    .Protected()
                    .Verify(
                        "SendAsync",
                        Times.Once(),
                        ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUri),
                        ItExpr.IsAny<CancellationToken>()
                    );
            }

            [Fact]
            public async Task Should_not_call_http_client_if_response_is_cached()
            {
                // arrange

                string            cuit              = "5050505050";
                string            cbu               = "2323232323";
                CancellationToken cancellationToken = CancellationToken.None;

                BankAccountOwner expected = new BankAccountOwner()
                {
                    valid = true
                };

                string url = $"BankAccountOwnership?account_address={cbu}&owner_key={cuit}";

                object value = expected;

                this.CacheAccessorMock
                    .Setup(x => x.TryGetValue(It.Is<string>(u => u.ToString() == url), out value))
                    .Returns(true);

                // act

                var actual = await this.Sut.GetBankAccountOwnershipAsync(cbu, cuit, cancellationToken);

                // assert

                actual.Should().BeEquivalentTo(expected);

                this.HttpMessageHandlerMock
                    .Protected()
                    .Verify(
                        "SendAsync",
                        Times.Never(),
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>()
                    );

                this.CacheAccessorMock.Verify(
                    x => x.TryGetValue(
                        It.IsAny<string>(),
                        out value
                    ),
                    Times.Once()
                );
            }

            [Fact]
            public async Task Should_cache_response_when_response_is_successful()
            {
                // arrange

                string            cuit              = "5050505050";
                string            cbu               = "2323232323";
                CancellationToken cancellationToken = CancellationToken.None;

                BankAccountOwner expected = new BankAccountOwner()
                {
                    valid = true
                };

                string url = $"BankAccountOwnership?account_address={cbu}&owner_key={cuit}";

                object value = null;

                this.CacheAccessorMock
                    .Setup(x => x.TryGetValue(It.Is<string>(u => u.ToString() == url), out value))
                    .Returns(false);

                this.HttpMessageHandlerMock
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(expected))
                    });

                // act

                var actual = await this.Sut.GetBankAccountOwnershipAsync(cbu, cuit, cancellationToken);

                // assert

                actual.Should().BeEquivalentTo(expected);

                this.HttpMessageHandlerMock
                    .Protected()
                    .Verify(
                        "SendAsync",
                        Times.Once(),
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>()
                    );

                this.CacheAccessorMock.Verify(
                    x => x.Set(
                        It.Is<string>(p => p == url),
                        It.IsAny<object>(),
                        this.CacheOptions.Value.EntityDataBasicExpiration
                    ),
                    Times.Once()
                );
            }
        }

        public class The_Method_GetSpecialRecordsAsync : PidPluginClientTests
        {
            [Fact]
            public async Task Should_call_http_client_with_correct_url_if_response_is_not_cached()
            {
                // arrange

                string            cuit              = "5050505050";
                string            rule              = "2323232323";
                CancellationToken cancellationToken = CancellationToken.None;

                SpecialRecordEntry expected = new SpecialRecordEntry()
                {
                    Status = "activo"
                };

                string url = $"SpecialRecord?key={cuit}&rule={rule}";

                Uri expectedUri = new Uri($"{this.HttpClient.BaseAddress}{url}");

                this.HttpMessageHandlerMock
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(expected))
                    });

                // act

                var actual = await this.Sut.GetSpecialRecordsAsync(cuit, rule, cancellationToken);

                // assert

                actual.Should().BeEquivalentTo(expected);

                this.HttpMessageHandlerMock
                    .Protected()
                    .Verify(
                        "SendAsync",
                        Times.Once(),
                        ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUri),
                        ItExpr.IsAny<CancellationToken>()
                    );
            }

            [Fact]
            public async Task Should_not_call_http_client_if_response_is_cached()
            {
                // arrange

                string            cuit              = "5050505050";
                string            rule              = "2323232323";
                CancellationToken cancellationToken = CancellationToken.None;

                SpecialRecordEntry expected = new SpecialRecordEntry()
                {
                    Status = "activo"
                };

                string url = $"SpecialRecord?key={cuit}&rule={rule}";

                object value = expected;

                this.CacheAccessorMock
                    .Setup(x => x.TryGetValue(It.Is<string>(u => u.ToString() == url), out value))
                    .Returns(true);

                // act

                var actual = await this.Sut.GetSpecialRecordsAsync(cuit, rule, cancellationToken);

                // assert

                actual.Should().BeEquivalentTo(expected);

                this.HttpMessageHandlerMock
                    .Protected()
                    .Verify(
                        "SendAsync",
                        Times.Never(),
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>()
                    );

                this.CacheAccessorMock.Verify(
                    x => x.TryGetValue(
                        It.IsAny<string>(),
                        out value
                    ),
                    Times.Once()
                );
            }

            [Fact]
            public async Task Should_cache_response_when_response_is_successful()
            {
                // arrange

                string            cuit              = "5050505050";
                string            rule              = "2323232323";
                CancellationToken cancellationToken = CancellationToken.None;

                SpecialRecordEntry expected = new SpecialRecordEntry()
                {
                    Status = "activo"
                };

                string url = $"SpecialRecord?key={cuit}&rule={rule}";

                object value = null;

                this.CacheAccessorMock
                    .Setup(x => x.TryGetValue(It.Is<string>(u => u.ToString() == url), out value))
                    .Returns(false);

                this.HttpMessageHandlerMock
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(expected))
                    });

                // act

                var actual = await this.Sut.GetSpecialRecordsAsync(cuit, rule, cancellationToken);

                // assert

                actual.Should().BeEquivalentTo(expected);

                this.HttpMessageHandlerMock
                    .Protected()
                    .Verify(
                        "SendAsync",
                        Times.Once(),
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>()
                    );

                this.CacheAccessorMock.Verify(
                    x => x.Set(
                        It.Is<string>(p => p == url),
                        It.IsAny<object>(),
                        this.CacheOptions.Value.EntityDataBasicExpiration
                    ),
                    Times.Once()
                );
            }
        }
    }
}
