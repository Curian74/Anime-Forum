using System.Text.Json;

namespace Domain.Common.Extensions
{
    public static class DeserializeExtensions
    {
        private static JsonSerializerOptions defaultSettings = new JsonSerializerOptions
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
