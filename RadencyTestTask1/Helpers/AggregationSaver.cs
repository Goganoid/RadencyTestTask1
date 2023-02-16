using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RadencyTestTask1.Entities;

namespace RadencyTestTask1.Helpers;

public class AggregationSaver
{
    public int ParsedFiles  = 0;
    public  int ParsedLines  = 0;
    private int _foundErrors;
    private List<string> InvalidFiles  = new();

    private readonly string _baseOutputDir;
    private readonly string _outputDir;

    public AggregationSaver(string baseOutputDir)
    {
        _baseOutputDir = baseOutputDir;
        _outputDir = Path.Join(_baseOutputDir, GetDateDir());
    }
    private static string GetDateDir()
    {
        var date = DateOnly.FromDateTime(DateTime.Now);
        return date.ToString("yyyy-dd-MM");
    }

    private string GenerateLog()
    {
        var report = new StringBuilder();
        report.AppendLine($"parsed_files:{ParsedFiles}");
        report.AppendLine($"parsed_lines:{ParsedLines}");
        report.AppendLine($"found_errors:{_foundErrors}");
        report.AppendLine($"invalid_files:[{string.Join(',',InvalidFiles)}]");
        return report.ToString();
    }
    public async Task SaveLog()
    {
        var log = GenerateLog();
        Directory.CreateDirectory(_outputDir);
        var filePath = Path.Join(_outputDir,"meta.log");
        await File.WriteAllTextAsync(filePath, log);
    }
    public async Task SaveTo(Task<AggregationResult> aggregationTask,string baseFilePath,CancellationToken token)
    {
        var aggregationResult = await aggregationTask;
        if (aggregationResult.InvalidLines > 0)
        {
            _foundErrors+=aggregationResult.InvalidLines;
            InvalidFiles.Add(baseFilePath);
        }
        DefaultContractResolver contractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        };
        if (token.IsCancellationRequested) return;
        var serialized = JsonConvert.SerializeObject(aggregationResult.Aggregation, new JsonSerializerSettings
        {
            ContractResolver = contractResolver,
            Formatting = Formatting.Indented
        });
        Directory.CreateDirectory(_outputDir);
        var filePath = Path.Join(_outputDir, Path.GetFileNameWithoutExtension(baseFilePath) + ".json");
        await File.WriteAllTextAsync(filePath, serialized);
        await SaveLog();
    }
}