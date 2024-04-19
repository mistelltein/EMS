using System.Text.Json;

namespace EMS.ClientLibrary.Helpers
{
    public static class Serializations
    {
        public static string SezializeObj<T>(T modeObject) =>
            JsonSerializer.Serialize(modeObject, typeof(T));

        public static T DeserializeJsonString<T>(string jsonString) =>
            JsonSerializer.Deserialize<T>(jsonString)!;

        public static IList<T> DeserializeJsonStringList<T>(string jsonString) =>
            JsonSerializer.Deserialize<IList<T>>(jsonString)!;
    }
}