using System.Collections.Generic;
using Flurl.Http;

namespace AzureDevOpsRest
{
    public interface IEnumerableRequest<TData>
    {
        IRequest<TData> Request { get; }
        IAsyncEnumerable<TData> Enumerator(IFlurlRequest request);
    }
}