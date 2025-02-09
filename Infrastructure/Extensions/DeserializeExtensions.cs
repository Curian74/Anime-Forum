using System.Text.Json;

namespace Infrastructure.Extensions
{
    public static class DeserializeExtensions
    {
        private static readonly JsonSerializerOptions defaultSettings = new()
        {
            PropertyNameCaseInsensitive = true, //Case insensitive moi lay duoc dung properties trong json
        };

        //Goi thang nay moi khi can Deserialize
        public static T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, defaultSettings)!; //Giong cai co san
        }
    }
}
