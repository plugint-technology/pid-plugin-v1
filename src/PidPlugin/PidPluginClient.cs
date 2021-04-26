using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using PidPlugin.Dtos;
using PidPlugin.Cache;
using PidPlugin.Extensions;
using Microsoft.Extensions.Options;

namespace PidPlugin
{
    public class PidPluginClient : IPidPluginClient
    {
        protected readonly HttpClient               httpClient;
        protected readonly ICacheAccessor           cacheAccessor;
        protected readonly IOptions<CacheOptions>   options;

        protected static SemaphoreSlim EntityBasicDataSemaphore      = new SemaphoreSlim(1);
        protected static SemaphoreSlim EntityFullDataSemaphore       = new SemaphoreSlim(1);
        protected static SemaphoreSlim SpecialRecordSemaphore        = new SemaphoreSlim(1);
        protected static SemaphoreSlim BankAccountDetailSemaphore    = new SemaphoreSlim(1);
        protected static SemaphoreSlim BankAccountOwnershipSemaphore = new SemaphoreSlim(1);

        public PidPluginClient(HttpClient httpClient, ICacheAccessor cacheAccessor, IOptions<CacheOptions> options)
        {
            this.httpClient = httpClient ??
                throw new ArgumentNullException(nameof(httpClient));

            this.cacheAccessor = cacheAccessor ??
                throw new ArgumentNullException(nameof(cacheAccessor));

            this.options = options ??
                throw new ArgumentNullException(nameof(options));
        }

        public async Task<EntityBasicData> GetEntityDataBasicAsync(string cuit, CancellationToken cancellationToken = default)
        {
            string url = $"EntityDataBasic?key={cuit}";
            TimeSpan expiration = this.options.Value.EntityDataBasicExpiration;

            return await GetFromCacheOrMakeRequestAsync<EntityBasicData>(
                url, EntityBasicDataSemaphore, expiration, cancellationToken);
        }

        public async Task<EntityFullData> GetEntityDataFullAsync(string cuit, CancellationToken cancellationToken = default)
        {
            string url = $"EntityDataFull?key={cuit}";
            TimeSpan expiration = this.options.Value.EntityDataFullExpiration;

            return await GetFromCacheOrMakeRequestAsync<EntityFullData>(
                url, EntityFullDataSemaphore, expiration, cancellationToken);
        }

        public async Task<SpecialRecordEntry> GetSpecialRecordsAsync(string cuit, string rule, CancellationToken cancellationToken = default)
        {
            string url = $"SpecialRecord?key={cuit}&rule={rule}";
            TimeSpan expiration = this.options.Value.SpecialRecordsExpiration;

            return await GetFromCacheOrMakeRequestAsync<SpecialRecordEntry>(
                url, SpecialRecordSemaphore, expiration, cancellationToken);
        }

        public async Task<BankAccountDetail> GetBankAccountDetailAsync(string cbu, CancellationToken cancellationToken = default)
        {
            string url = $"BankAccountDetails?key={cbu}";
            TimeSpan expiration = this.options.Value.BankAccountDetailExpiration;

            return await GetFromCacheOrMakeRequestAsync<BankAccountDetail>(
                url, BankAccountDetailSemaphore, expiration, cancellationToken);
        }

        public async Task<BankAccountOwner> GetBankAccountOwnershipAsync(string cbu, string cuit, CancellationToken cancellationToken = default)
        {
            string url = $"BankAccountOwnership?account_address={cbu}&owner_key={cuit}";
            TimeSpan expiration = this.options.Value.BankAccountOwnershipExpiration;

            return await GetFromCacheOrMakeRequestAsync<BankAccountOwner>(
                url, BankAccountOwnershipSemaphore, expiration, cancellationToken);
        }

        private async Task<TResponse> GetFromCacheOrMakeRequestAsync<TResponse>(string url, SemaphoreSlim semaphore, TimeSpan expiration, CancellationToken cancellationToken = default)
        {
            if (expiration.TotalMinutes <= 0)
                return await this.httpClient.GetAsync<TResponse>(url, cancellationToken);

            if (this.cacheAccessor.TryGetValue(url, out object value))
                return (TResponse)value;

            try
            {
                await semaphore.WaitAsync();

                if (this.cacheAccessor.TryGetValue(url, out value))
                    return (TResponse)value;

                TResponse response = await this.httpClient
                    .GetAsync<TResponse>(url, cancellationToken);

                this.cacheAccessor.Set(url, response, expiration);

                return response;
            }
            catch
            {
                throw;
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
