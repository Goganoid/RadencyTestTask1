namespace RadencyTestTask1.Entities;

public class Service
{
    public required string Name;
    public List<Payer> Payers = new();
    public decimal Total;
}