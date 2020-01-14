using System.Threading.Tasks;

namespace AzureDevOpsRest
{
    public interface IClient
    {
        Task<TData> GetAsync<TData>(IRequest<TData> request);
    }
}