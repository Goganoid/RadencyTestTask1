using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RadencyTestTask1.Entities;

namespace RadencyTestTask1.Helpers;

public class AggregationSaver
{
    private static int _counter = 0;
    public int ParsedFiles  = 0;
    public  int ParsedLines  = 0;
    private int _foundErrors;
    private readonly List<string> _invalidFiles  = new();

    private readonly string _outputDir;
    private readonly string _logPath;

    public AggregationSaver(string baseOutputDir)
    {
        _outputDir = Path.Join(baseOutputDir, GetDateDir());
        _logPath = Path.Join(_outputDir,"meta.log");
        if (File.Exists(_logPath))
        {
            RecoverLogs();
        }
       
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
        report.AppendLine($"invalid_files:[{string.Join(',',_invalidFiles)}]");
        return report.ToString();
    }

    private void RecoverLogs()
    {
        var content = File.ReadAllLines(_logPath);
        var props = content.Select(line => line.Split(':')[1]).ToList();
        ParsedFiles += int.TryParse(props[0], out var parsedFiles) ? parsedFiles : 0;
        ParsedLines += int.TryParse(props[1], out var parsedLines) ? parsedLines : 0;
        _foundErrors += int.TryParse(props[2], out var foundErrors) ? foundErrors : 0;
        var invalidFiles = props[3]
            .Replace("[", "")
            .Replace("]", "")
            .Split(",");
        foreach (var invalidFile in invalidFiles)
        {
            _invalidFiles.Add(invalidFile);
        }
    }
    public async Task SaveLog()
    {
        var log = GenerateLog();
        Directory.CreateDirectory(_outputDir);
        await File.WriteAllTextAsync(_logPath, log);
    }
    public async Task SaveTo(Task<AggregationResult> aggregationTask,string baseFilePath,CancellationToken token)
    {
        var aggregationResult = await aggregationTask;
        if (aggregationResult.InvalidLines > 0)
        {
            _foundErrors+=aggregationResult.InvalidLines;
            _invalidFiles.Add(baseFilePath);
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
        var filePath = Path.Join(_outputDir, $"output{_counter++}");
        await File.WriteAllTextAsync(filePath, serialized,token);
        await SaveLog();
    }
}