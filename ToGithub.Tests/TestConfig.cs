using Microsoft.Extensions.Configuration;

namespace ToGithub.Tests
{
    public class TestConfig
    {
        public AzureDevops AzDo { get; set; }
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

    public class AzureDevops
    {
        public string Organization { get; set; }
        public string Token { get; set; }
    }
}