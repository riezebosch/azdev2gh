using System.Collections.Generic;

namespace AzureDevOpsRestClient
{
    public interface IRequest<TData>
    {
        string Resource { get; }
        IDictionary<string, object> QueryParams { get; }
        string BaseUrl(string organization);
    }
}