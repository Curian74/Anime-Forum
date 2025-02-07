using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
