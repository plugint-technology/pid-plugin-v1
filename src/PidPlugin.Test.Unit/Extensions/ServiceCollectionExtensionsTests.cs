using System;
using System.Net.Http;
using PidPlugin.Cache;
using PidPlugin.Extensions;
using PidPlugin.Handlers;
using PidPlugin.Settings;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Linq;
using Xunit;

namespace PidPlugin.Test.Unit.Extensions
{
    public class ServiceCollectionExtensionsTests
    {
        private IServiceCollection Sut { get; set; }

        public ServiceCollectionExtensionsTests()
        {
            this.Sut = new ServiceCollection();
        }

        public class TheMethod_AddPidPlugin : ServiceCollectionExtensionsTests
        {
            private PidPluginSettings Settings { get; set; }

            public TheMethod_AddPidPlugin()
            {
                this.Settings = new PidPluginSettings()
                {
                    BaseUrl          = "https://fake-url.com/api/",
                    RetryAttempts    = 3,
                    SubscriptionKey  = "",
                    TimeoutInMinutes = 2,
                    CacheSettings = new CacheSettings()
                    {
                        BankAccountDetailInMinutes    = 1,
                        SpecialRecordsInMinutes       = 2,
                        EntityDataBasicInMinutes      = 3,
                        BankAccountOwnershipInMinutes = 4,
                        EntityDataFullInMinutes       = 5
                    }
                };
            }

            [Fact]
            public void Should_throw_an_ArgumentNullException_when_pid_plugin_settings_is_null()
            {
                // arrange
                PidPluginSettings pidPluginSettings = null;

                // act & assert
                Assert.Throws<ArgumentNullException>(nameof(pidPluginSettings), () => this.Sut.AddPidPlugin(pidPluginSettings));
            }

            [Fact]
            public void Should_add_cache_options()
            {
                // arrange

                TimeSpan bankAccountDetailExpirationExpected = 
                    TimeSpan.FromMinutes(this.Settings.CacheSettings.BankAccountDetailInMinutes);

                TimeSpan specialRecordsExpirationExpected =
                    TimeSpan.FromMinutes(this.Settings.CacheSettings.SpecialRecordsInMinutes);

                TimeSpan entityDataBasicExpirationExpected =
                    TimeSpan.FromMinutes(this.Settings.CacheSettings.EntityDataBasicInMinutes);

                TimeSpan bankAccountOwnershipExpirationExpected =
                    TimeSpan.FromMinutes(this.Settings.CacheSettings.BankAccountOwnershipInMinutes);

                TimeSpan entityDataFullExpirationExpected =
                    TimeSpan.FromMinutes(this.Settings.CacheSettings.EntityDataFullInMinutes);

                // act

                this.Sut.AddPidPlugin(this.Settings);

                // assert

                var options = this.Sut.BuildServiceProvider()
                    .GetService<IOptions<CacheOptions>>();

                options.Should().NotBeNull();
                options.Value.Should().NotBeNull();

                options.Value.BankAccountDetailExpiration.Should().Be(bankAccountDetailExpirationExpected);
                options.Value.SpecialRecordsExpiration.Should().Be(specialRecordsExpirationExpected);
                options.Value.EntityDataBasicExpiration.Should().Be(entityDataBasicExpirationExpected);
                options.Value.BankAccountOwnershipExpiration.Should().Be(bankAccountOwnershipExpirationExpected);
                options.Value.EntityDataFullExpiration.Should().Be(entityDataFullExpirationExpected);
            }

            [Fact]
            public void Should_add_memory_cache()
            {
                // arrange

                // act

                this.Sut.AddPidPlugin(this.Settings);

                // assert

                this.Sut.Should().Contain(descriptor =>
                    descriptor.Lifetime.Equals(ServiceLifetime.Singleton) &&
                    descriptor.ServiceType.Equals(typeof(IMemoryCache)));
            }

            [Fact]
            public void Should_add_cache_accessor()
            {
                // arrange

                // act

                this.Sut.AddPidPlugin(this.Settings);

                // assert

                this.Sut.Should().Contain(descriptor =>
                    descriptor.Lifetime.Equals(ServiceLifetime.Transient) &&
                    descriptor.ServiceType.Equals(typeof(ICacheAccessor)));
            }

            [Fact]
            public void Should_add_http_client_error_handler()
            {
                // arrange

                // act

                this.Sut.AddPidPlugin(this.Settings);

                // assert

                this.Sut.Should().Contain(descriptor =>
                    descriptor.Lifetime.Equals(ServiceLifetime.Transient) &&
                    descriptor.ServiceType.Equals(typeof(HttpClientErrorHandler)));
            }

            [Fact]
            public void Should_add_pid_plugin_client_with_supplied_base_address()
            {
                // arrange

                Uri expected = new Uri(this.Settings.BaseUrl);

                // act

                this.Sut.AddPidPlugin(this.Settings);

                // assert

                this.Sut.Should().Contain(descriptor =>
                    descriptor.Lifetime.Equals(ServiceLifetime.Transient) &&
                    descriptor.ServiceType.Equals(typeof(HttpClientErrorHandler)));

                var client = this.Sut.BuildServiceProvider()
                    .GetService<IPidPluginClient>();

                var httpClient = (HttpClient)client.GetType()
                    .GetField("httpClient", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(client);

                httpClient.BaseAddress.Should().Be(expected);
            }

            [Fact]
            public void Should_add_pid_plugin_client_with_supplied_timeout()
            {
                // arrange

                TimeSpan expected = TimeSpan.FromMinutes(this.Settings.TimeoutInMinutes);

                // act

                this.Sut.AddPidPlugin(this.Settings);

                // assert

                this.Sut.Should().Contain(descriptor =>
                    descriptor.Lifetime.Equals(ServiceLifetime.Transient) &&
                    descriptor.ServiceType.Equals(typeof(HttpClientErrorHandler)));

                var client = this.Sut.BuildServiceProvider()
                    .GetService<IPidPluginClient>();

                var httpClient = (HttpClient)client.GetType()
                    .GetField("httpClient", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(client);

                httpClient.Timeout.Should().Be(expected);
            }

            [Fact]
            public void Should_add_pid_plugin_client_with_supplied_auth_header()
            {
                // arrange

                string expectedHeaderValue = this.Settings.SubscriptionKey;

                // act

                this.Sut.AddPidPlugin(this.Settings);

                // assert

                this.Sut.Should().Contain(descriptor =>
                    descriptor.Lifetime.Equals(ServiceLifetime.Transient) &&
                    descriptor.ServiceType.Equals(typeof(HttpClientErrorHandler)));

                var client = this.Sut.BuildServiceProvider()
                    .GetService<IPidPluginClient>();

                var httpClient = (HttpClient)client.GetType()
                    .GetField("httpClient", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(client);

                httpClient.DefaultRequestHeaders.Should().HaveCount(1);
                
                httpClient.DefaultRequestHeaders.GetValues("Ocp-Apim-Subscription-Key")
                    .First().Should().Be(expectedHeaderValue);
            }

            [Fact]
            public void Should_return_the_same_instance_as_Sut()
            {
                // arrange

                // act
                IServiceCollection actual = this.Sut.AddPidPlugin(this.Settings);

                // assert
                actual.Should().BeSameAs(this.Sut);
            }
        }
    }
}
