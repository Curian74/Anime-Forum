namespace Domain.Interfaces
{
    public interface IConfigurationManager
    {
        string GetValue(string key);
        string GetConnectionString(string name);
        T GetSection<T>(string sectionName) where T : class, new();
    }
}
