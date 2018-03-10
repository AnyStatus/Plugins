using AnyStatus.API;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class HTTPHealthCheck : ICheckHealth<HttpStatus>
    {
        [DebuggerStepThrough]
        public async Task Handle(HealthCheckRequest<HttpStatus> request, CancellationToken cancellationToken)
        {
            using (var handler = new WebRequestHandler())
            {
                if (request.DataContext.IgnoreSslErrors)
                    handler.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

                if (request.DataContext.UseDefaultCredentials)
                    handler.UseDefaultCredentials = true;

                try
                {
                    using (var client = new HttpClient(handler))
                    {
                        var response = await client.GetAsync(request.DataContext.Url).ConfigureAwait(false);

                        request.DataContext.State = response.StatusCode == request.DataContext.HttpStatusCode ? State.Ok : State.Failed;
                    }
                }
                catch (AggregateException ae)
                {
                    ae.Handle(ex =>
                    {
                        if (ex is HttpRequestException)
                            request.DataContext.State = State.Failed;

                        return ex is HttpRequestException;
                    });
                }
            }
        }
    }
}