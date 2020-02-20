using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using ToGithub;

namespace Functions.Activities
{
    public class GetProductBacklogItems
    {
        private readonly Func<AzureDevOpsData, IFromAzureDevOps> _source;

        public GetProductBacklogItems(Func<AzureDevOpsData, IFromAzureDevOps> source)
        {
            _source = source;
        }

        [FunctionName(nameof(GetProductBacklogItems))]
        public IEnumerable<int> Run([ActivityTrigger]AzureDevOpsData input)
        {
            return _source(input).ProductBacklogItems(input.AreaPath).ToEnumerable().Take(50);
        }
    }
}