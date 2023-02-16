using RadencyTestTask1.Entities;

namespace RadencyTestTask1.Helpers;
/// <summary>
/// This class is responsible for aggregating and parsing lines
/// </summary>
public static class Aggregator
{
    private static List<City> AggregateCities(IEnumerable<InputLine> lines)
    {
        return lines
            .GroupBy(l => l.City)
            .Select(cityGroup => new City
            {
                CityName = cityGroup.Key,
                Total = cityGroup.Sum(line => line.Payment),
                Services = AggregateServices(cityGroup)
            }).ToList();
    }

    private static List<Service> AggregateServices(IEnumerable<InputLine> cityGroup)
    {
        return cityGroup.GroupBy(g => g.Service)
            .Select(serviceGroup => new Service
            {
                Name = serviceGroup.Key,
                Total = serviceGroup.Sum(line => line.Payment),
                Payers = AggregatePayers(serviceGroup)
            }).ToList();
    }

    private static List<Payer> AggregatePayers(IEnumerable<InputLine> serviceGroup)
    {
        return serviceGroup.Select(line => new Payer
        {
            Name = $"{line.FirstName} {line.LastName}",
            Date = line.Date,
            Payment = line.Payment,
            AccountNumber = line.AccountNumber
        }).ToList();
    }
    /// <summary>
    /// Parses and aggregates a batch of lines
    /// </summary>
    /// <param name="lines">String lines</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static AggregationResult AggregateLines(IEnumerable<string> lines, CancellationToken cancellationToken)
    {
        List<InputLine> inputLines = new();
        var invalidLines = 0;
        foreach (var line in lines)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("Stopped aggregation process");
                return new AggregationResult {Aggregation = new(), InvalidLines = 0};
            }
            var elements = line
                .Replace("\"", "")
                .Split(',')
                .Select(l=>l.Trim())
                .ToList();
            if (elements.Count != 9)
            {
                invalidLines++;
            }
            else
            {
                if (!decimal.TryParse(elements[5], out var payment)
                    || !DateOnly.TryParseExact(elements[6],"yyyy-dd-MM", out var date)
                    || !long.TryParse(elements[7], out var accountNumber))
                {
                    invalidLines++;
                    continue;
                }

                inputLines.Add(new InputLine
                {
                    FirstName = elements[0],
                    LastName = elements[1],
                    City = elements[2],
                    Payment = payment,
                    Date = date,
                    AccountNumber = accountNumber,
                    Service = elements[8]
                });
            }
        }
        
        var aggregation = AggregateCities(inputLines);
        return new AggregationResult
        {
            Aggregation = aggregation,
            InvalidLines = invalidLines,
        };
    }

    /// <summary>
    /// Aggregates all smaller aggregations into one
    /// </summary>
    /// <param name="tasks">Aggregations</param>
    /// <returns></returns>
    public static async Task<AggregationResult> JoinAll(List<Task<AggregationResult>> tasks)
    {
        var aggregationResults = await Task.WhenAll(tasks);
        var aggregation = aggregationResults
            .Select(a=>a.Aggregation)
            .SelectMany(x => x)
            .GroupBy(x => x.CityName)
            .Select(cityGroup => new City
            {
                CityName = cityGroup.Key,
                Total = cityGroup.Sum(c => c.Total),
                Services = JoinServices(cityGroup)
            }).ToList();
        var invalidLines = aggregationResults.Select(a => a.InvalidLines).Sum();
        return new AggregationResult
        {
            Aggregation =  aggregation,
            InvalidLines = invalidLines,
        };
    }

    private static List<Service> JoinServices(IEnumerable<City> cityGroup)
    {
        return cityGroup
            .SelectMany(c => c.Services)
            .GroupBy(s => s.Name)
            .Select(serviceGroup => new Service
            {
                Name = serviceGroup.Key,
                Total = serviceGroup.Sum(s => s.Total),
                Payers = serviceGroup.SelectMany(p => p.Payers).ToList()
            }).ToList();
    }

}