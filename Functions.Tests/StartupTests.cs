using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Functions.Tests
{
    public static class StartupTests
    {
        [Fact]
        public static void Test()
        {
            var startup = new Startup();

            var services = new ServiceCollection();

            var builder = new Mock<IWebJobsBuilder>();
            builder
                .Setup(x => x.Services)
                .Returns(services);

            var functions = startup
                .GetType()
                .Assembly
                .GetTypes()
                .Where(type => type.GetMethods().Any(method =>
                    method.GetCustomAttributes(typeof(FunctionNameAttribute), false).Any() &&
                    !method.IsStatic))
                .ToList();

            functions.ForEach(f => services.AddScoped(f));

            startup.Configure(builder.Object);
            var provider = services.BuildServiceProvider();

            functions.ForEach(f => provider.GetService(f));
        }
    }
}