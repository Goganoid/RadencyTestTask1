using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RadencyTestTask1.FileProcessing;

namespace RadencyTestTask1.Entities;
/// <summary>
/// The config file is deserialized to this object
/// </summary>
public class AppConfig
{
    public required string OutputDirectory { get; set; }
    public required string WatchDirectory { get; set; }
    public required uint ChunkSize { get; set; }
    [JsonConverter(typeof(StringEnumConverter))]
    public ProcessingOptions Behavior { get; set; }
}