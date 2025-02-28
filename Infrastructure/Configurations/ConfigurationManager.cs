using Microsoft.Extensions.Configuration;

namespace Infrastructure.Configurations 
{ 
    public class ConfigurationManager(IConfiguration configuration) : Domain.Interfaces.IConfigurationManager
    {
        private readonly IConfiguration _configuration = configuration;

        public string GetValue(string key)
        {
            return _configuration[key];
        }

        public string GetConnectionString(string name)
        {
            return _configuration.GetConnectionString(name);
        }

        public T GetSection<T>(string sectionName) where T : class, new()
        {
            var section = new T();
            _configuration.GetSection(sectionName).Bind(section);
            return section;
        }
    }
}

