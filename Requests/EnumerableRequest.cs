using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using AzureDevOpsRest.Data;
using Flurl.Http;

namespace AzureDevOpsRest.Requests
{
    public class EnumerableRequest<TData> : Request<TData>, IEnumerableRequest<TData>
    {
        public EnumerableRequest(string resource, string api) : base(resource, api)
        {
        }

        public async IAsyncEnumerable<TData> Enumerator(IFlurlRequest request)
        {
            bool more;
            do
            {
                var task = request.GetAsync();
                more = HandleContinuation(request, await task);

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