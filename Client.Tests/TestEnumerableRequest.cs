using System.Collections.Generic;
using System.Linq;
using Flurl.Http;

namespace AzureDevOpsRest.Tests
{
    internal class TestEnumerableRequest : IEnumerableRequest<object>
    {
        public TestEnumerableRequest(string resource) => 
            Request = new TestRequest(resource);

        public IRequest<object> Request { get; }
        public IAsyncEnumerable<object> Enumerator(IFlurlRequest request) => 
            request.GetJsonAsync<object>().ToAsyncEnumerable();
    }
}