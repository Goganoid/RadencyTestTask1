namespace RadencyTestTask1.FileProcessing;

public class FileProcessor
{
    public string Path { get; set; }
    private ProcessStrategy? ProcessStrategy { get; set; }

    public FileProcessor(string path, ProcessingOptions processingOptions)
    {
        Path = path;
        ProcessStrategy = processingOptions switch
        {
            ProcessingOptions.ReadOnce => new ReadOnceStrategy(),
            ProcessingOptions.ReadContinously => new ReadContinouslyStrategy(),
            _ => null,
        };
    }

    public void Run()
    {
        ProcessStrategy?.ProcessDirectory(Path);
    }
}