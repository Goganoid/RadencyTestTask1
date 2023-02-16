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
/// <summary>
/// Abstract class for other strategies and with default ProcessFile implementation
/// </summary>
public abstract class ProcessStrategy
{
    protected AppConfig Config;
    protected CancellationTokenSource TokenSource;
    protected AggregationSaver AggregationSaver { get; set; }
    protected  List<Task> SaveTasks;

    protected ProcessStrategy(AppConfig config)
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
        try
        {
            foreach (var line in documentReader.GetLines())
            {
                if (TokenSource.IsCancellationRequested)
                {
                    Console.WriteLine($"Stopped reading the {filePath}");
                    return;
                }

                AggregationSaver.ParsedLines++;
                chunk.Add(line);
                if (chunk.Count <= Config.ChunkSize) continue;
                var chunkCopy = chunk;
                aggregations.Add(Task.Run(() => Aggregator.AggregateLines(chunkCopy, TokenSource.Token)));
                chunk = new();
            }
        }
        catch (IOException exception)
        {
            Console.WriteLine($"File {filePath} is in use, skipping.");
            return;
        }
        if (chunk.Count > 0) aggregations.Add(Task.Run(()=>Aggregator.AggregateLines(chunk,TokenSource.Token)));
        SaveTasks.Add(
            AggregationSaver.SaveTo(
                Task.Run(()=>Aggregator.JoinAll(aggregations)),
                filePath,TokenSource.Token));
    }
}