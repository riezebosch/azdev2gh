namespace AzureDevOpsRestClient
{
    public static class RequestExtensions
    { 
        public static IEnumerableRequest<TData> WithQueryParams<TData>(this IEnumerableRequest<TData> request, params (string key, object value)[] items)
        {
            foreach (var (key, value) in items)
            {
                request.QueryParams[key] = value;
            }
            return request;
        }
    }
}