namespace RadencyTestTask1.LineReaders;

public interface ILineReader
{
    public StreamReader StreamReader { get;}
    public string? ReadLine();
}
