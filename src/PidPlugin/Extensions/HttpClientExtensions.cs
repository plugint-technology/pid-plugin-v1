using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PidPlugin.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<TResponse> GetAsync<TResponse>(this HttpClient httpClient, string url, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage httpResponseMessage =
                await httpClient.GetAsync(url, cancellationToken);

            string content = await httpResponseMessage.Content
                .ReadAsStringAsync();

            return JsonConvert
                .DeserializeObject<TResponse>(content);
        }
    }
}
