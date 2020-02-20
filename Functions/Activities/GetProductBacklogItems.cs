using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using ToGithub;

namespace Functions.Activities
{
    public class GetProductBacklogItems
    {
        private IFromAzureDevOps _source;

        public GetProductBacklogItems(IFromAzureDevOps source)
        {
            _source = source;
        }

        [FunctionName(nameof(GetProductBacklogItems))]
        public IEnumerable<int> Run([ActivityTrigger]string area) => 
            _source.ProductBacklogItems(area).ToEnumerable();
    }
}