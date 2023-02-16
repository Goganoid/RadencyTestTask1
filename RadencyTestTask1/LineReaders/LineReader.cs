namespace RadencyTestTask1.LineReaders;

public class LineReader : ILineReader
{
    public StreamReader StreamReader { get; private set; }
    public LineReader(string path)
    {
        StreamReader = new StreamReader(path);
    }
    public string? ReadLine()
    {
        return StreamReader.ReadLine();
    }
}