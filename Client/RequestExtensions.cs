using System;
using System.Diagnostics.CodeAnalysis;

namespace AzureDevOpsRest
{
    public static class RequestExtensions
    { 
        public static IEnumerableRequest<TData> WithQueryParams<TData>([NotNull]this IEnumerableRequest<TData> enumerable, params (string key, object value)[] items)
        {
            if (enumerable == null) throw new ArgumentNullException(nameof(enumerable));
            enumerable.Request.WithQueryParams(items);
            
            return enumerable;
        }

        public static IRequest<TData> WithQueryParams<TData>(this IRequest<TData> request, params (string key, object value)[] items)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            foreach (var (key, value) in items)
            {
                request.QueryParams[key] = value;
            }

            return request;
        }
    }
}