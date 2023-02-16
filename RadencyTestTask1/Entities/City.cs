using System.Text.Json.Serialization;

namespace RadencyTestTask1.Entities;

public class City
{
    [JsonPropertyName("city")]
    public required string CityName;
    public List<Service> Services = new();
    public decimal Total;
}