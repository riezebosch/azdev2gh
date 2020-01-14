using System.Collections.Generic;

namespace AzureDevOpsRest
{
    public interface IRequest<TData>
    {
        string Resource { get; }
        IDictionary<string, object> QueryParams { get; }
        string BaseUrl(string organization);
    }
}