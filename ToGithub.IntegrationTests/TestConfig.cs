using Microsoft.Extensions.Configuration;

namespace ToGithub.IntegrationTests
{
    public class TestConfig
    {
        public AzureDevOps AzureDevOps { get; set; }
        public GitHub GitHub { get; set; }        
       

        public TestConfig()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("settings.json", false)
                .AddJsonFile("settings.user.json", true)
                .Build();
            
            configuration.Bind(this);
        }
    }

    public class GitHub
    {
        public string Token { get; set; }
    }

    public class AzureDevOps
    {
        public string Organization { get; set; }
        public string Token { get; set; }
    }
}