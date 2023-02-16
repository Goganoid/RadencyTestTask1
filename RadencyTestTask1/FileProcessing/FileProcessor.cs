using RadencyTestTask1.Entities;

namespace RadencyTestTask1.FileProcessing;
/// <summary>
/// Responsible for choosing and using strategies
/// </summary>
public class FileProcessor
{
    private readonly AppConfig _config;
    private ProcessStrategy? ProcessStrategy { get; set; }

    public FileProcessor(AppConfig appConfig)
    {
        _config = appConfig;
        ProcessStrategy = _config.Behavior switch
        {
            ProcessingOptions.ReadOnce => new ReadOnceStrategy(appConfig),
            ProcessingOptions.ReadContinouosly => new ReadContinuouslyStrategy(appConfig),
            _ => null,
        };
    }

    public void Run()
    {
        if (!Directory.Exists(_config.WatchDirectory))
        {
            Console.WriteLine($"Creating directory {_config.WatchDirectory}");
            Directory.CreateDirectory(_config.WatchDirectory);
        }
        Console.WriteLine($"Using directory {_config.WatchDirectory}");
        ProcessStrategy?.ProcessDirectory(_config.WatchDirectory);
    }
}