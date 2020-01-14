using System.Collections.Generic;
using Flurl.Http;

namespace AzureDevOpsRest
{
    public interface IEnumerableRequest<TData> : IRequest<TData>
    {
        IAsyncEnumerable<TData> Enumerator(IFlurlRequest request);
    }
}