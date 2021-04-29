using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using PidPlugin.Dtos;
using PidPlugin.Exceptions;
using Newtonsoft.Json;

namespace PidPlugin.Handlers
{
    public class HttpClientErrorHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = 
                await base.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
                return response;
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new PidPluginUnauthorizedException();

            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new PidPluginNotFoundException();

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                string body = await response
                    .Content.ReadAsStringAsync();

                ErrorResponse errorResponse  = JsonConvert
                    .DeserializeObject<ErrorResponse>(body);

                throw new PidPluginValidationException(errorResponse.Message ?? errorResponse.Error);
            }

            throw new PidPluginException(response.ReasonPhrase);
        }
    }
}
