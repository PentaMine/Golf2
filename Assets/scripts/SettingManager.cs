using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class SettingManager
{
    public enum Language
    {
        ENGLISH = 0,
        CROATIAN = 1
    }

    public class Settings
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public Language lang { get; set; }

        public string server;
        public int port;
        public string authToken;

        public string getFullHTTPUri(string path)
        {
            return $"http://{server}:{port}{path}";
        }


        public string getWebSocketUri()
        {
            return $"ws://{server}:{port}";
        }
    }

    public static Settings settings = new Settings();

    public static async void SaveSettings()
    {
        string json = JsonConvert.SerializeObject(settings);
        await File.WriteAllTextAsync("./settings.json", json);
    }

    public static void LoadSettings()
    {
        string json = File.ReadAllText("./settings.json");
        settings = JsonConvert.DeserializeObject<Settings>(json);
    }
}