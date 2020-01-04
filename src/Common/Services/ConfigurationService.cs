using Microsoft.Extensions.Configuration;
using System.IO;

namespace Common.Services
{
    public interface IConfigurationService
    {
        IConfiguration Configuration { get; }
    }

    public class ConfigurationService : IConfigurationService
    {
        public IConfiguration Configuration => new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
    }
}
