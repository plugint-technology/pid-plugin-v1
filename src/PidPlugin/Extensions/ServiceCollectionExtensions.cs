using System;
using PidPlugin.Settings;
using PidPlugin.Handlers;
using PidPlugin.Cache;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace PidPlugin.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private const string SubscriptionKeyHeader = "Ocp-Apim-Subscription-Key";

        public static IServiceCollection AddPidPlugin(this IServiceCollection services, PidPluginSettings pidPluginSettings)
        {
            services.Configure<CacheOptions>(options =>
            {
                options.BankAccountDetailExpiration = TimeSpan.FromMinutes(
                    pidPluginSettings.CacheSettings.BankAccountDetailInMinutes);

                options.BankAccountOwnershipExpiration = TimeSpan.FromMinutes(
                    pidPluginSettings.CacheSettings.BankAccountOwnershipInMinutes);

                options.EntityDataBasicExpiration = TimeSpan.FromMinutes(
                    pidPluginSettings.CacheSettings.EntityDataBasicInMinutes);

                options.EntityDataFullExpiration = TimeSpan.FromMinutes(
                    pidPluginSettings.CacheSettings.EntityDataFullInMinutes);

                options.SpecialRecordsExpiration = TimeSpan.FromMinutes(
                    pidPluginSettings.CacheSettings.SpecialRecordsInMinutes);
            });

            services.AddMemoryCache();
            services.AddSingleton<ICacheAccessor, MemoryCacheAccessor>();
            services.AddHttpClient();
            services.AddTransient<ExceptionHandler>();

            services.AddHttpClient<IPidPluginClient, PidPluginClient>()
                .ConfigureHttpClient(builder =>
                {
                    builder.BaseAddress = 
                        new Uri(pidPluginSettings.BaseUrl);

                    builder.Timeout = 
                        TimeSpan.FromMinutes(pidPluginSettings.TimeoutInMinutes);

                    builder.DefaultRequestHeaders.Add(
                        SubscriptionKeyHeader, 
                        pidPluginSettings.SubscriptionKey
                    );
                })
                .AddHttpMessageHandler<ExceptionHandler>()
                .AddTransientHttpErrorPolicy(policy =>
                {
                    return policy.WaitAndRetryAsync(
                        pidPluginSettings.RetryAttempts, 
                        _ => TimeSpan.FromMilliseconds(200)
                    );
                });

            return services;
        }
    }
}
