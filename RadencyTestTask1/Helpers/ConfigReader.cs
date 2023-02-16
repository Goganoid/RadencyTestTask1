using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RadencyTestTask1.Entities;

namespace RadencyTestTask1.Helpers;

public static class ConfigReader
{
    public static AppConfig ReadConfig(string path="./config.json")
    {
        if (!File.Exists(path)) throw new Exception("Config wasn't found");
        List<string> parsingErrors = new();
        var settings = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Error,
            ContractResolver = new DefaultContractResolver{NamingStrategy = new SnakeCaseNamingStrategy()}
        };
        try
        {
            return JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(path), settings)!;
        }
        catch (JsonException exception)
        {
            throw new JsonException(exception.Message);
        }
       
    }
}