using System.Runtime.Serialization;
using RadencyTestTask1.DocumentReaders;
using RadencyTestTask1.Entities;
using RadencyTestTask1.Helpers;

namespace RadencyTestTask1.FileProcessing;
public enum ProcessingOptions
{
    [EnumMember(Value = "ReadOnce")]
    ReadOnce,
    [EnumMember(Value = "ReadContinuously")]
    ReadContinouosly,
}
public abstract class ProcessStrategy
{
    public AppConfig Config;
    public CancellationTokenSource TokenSource;
    protected AggregationSaver AggregationSaver { get; set; }
    protected  List<Task> SaveTasks;

    public ProcessStrategy(AppConfig config)
    {
        Config = config;
        TokenSource = new CancellationTokenSource();
        AggregationSaver = new (Config.OutputDirectory);
        SaveTasks = new();
    }
    public abstract void ProcessDirectory(string path);

    protected void ProcessFile(string filePath)
    {
        var aggregations = new List<Task<AggregationResult>>();
        var chunk = new List<string>();
        DocumentReader documentReader = Path.GetExtension(filePath) switch
        {
            ".csv" => new CsvDocumentReader(filePath),
            ".txt" => new TxtDocumentReader(filePath),
            _ => throw new NotImplementedException(),
        };
        AggregationSaver.ParsedFiles++;
        foreach (var line in documentReader.GetLines())
        {
            if(TokenSource.IsCancellationRequested)
            {
                Console.WriteLine($"Stopped reading the {filePath}");
                return;
            }
            AggregationSaver.ParsedLines++;
            chunk.Add(line);
            if (chunk.Count <= Config.ChunkSize) continue;
            var chunkCopy = chunk;
            aggregations.Add(Task.Run(() => Aggregator.AggregateLines(chunkCopy,filePath,TokenSource.Token)));
            chunk = new();
        }
        if (chunk.Count > 0) aggregations.Add(Task.Run(()=>Aggregator.AggregateLines(chunk,filePath,TokenSource.Token)));
        SaveTasks.Add(
            AggregationSaver.SaveTo(
                Task.Run(()=>Aggregator.JoinAll(aggregations)),
                filePath,TokenSource.Token));
    }
}