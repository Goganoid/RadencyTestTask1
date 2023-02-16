using System.Text.Json.Serialization;

namespace RadencyTestTask1.Entities;

public class City
{
    [JsonPropertyName("city")]
    public string CityName;
    public List<Service> Services = new();
    public decimal Total;
}