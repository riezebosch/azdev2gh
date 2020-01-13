using System.Threading.Tasks;

namespace AzureDevOpsRestClient
{
    public interface IClient
    {
        Task<TData> GetAsync<TData>(IRequest<TData> request);
    }
}