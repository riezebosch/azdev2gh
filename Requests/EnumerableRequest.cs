using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using AzureDevOpsRest.Data;
using Flurl.Http;

namespace AzureDevOpsRest.Requests
{
    public class EnumerableRequest<TData> : IEnumerableRequest<TData>
    {
        public EnumerableRequest(Request<TData> request) =>
            Request = request;

        public IRequest<TData> Request { get; }

        public async IAsyncEnumerable<TData> Enumerator(IFlurlRequest request)
        {
            bool more;
            do
            {
                var task = request.GetAsync();
                more = HandleContinuation(request, await task.ConfigureAwait(false));

                var items = await task.ReceiveJson<Multiple<TData>>();
                foreach (var item in items.Value)
                {
                    yield return item;
                }
            } while (more);
        }

        private static bool HandleContinuation(IFlurlRequest request, HttpResponseMessage response)
        {
            var more = response.Headers.TryGetValues("x-ms-continuationtoken", out var tokens);
            request.SetQueryParam("continuationToken", tokens?.First());
            
            return more;
        }
    }
}