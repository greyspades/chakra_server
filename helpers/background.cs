namespace TimedBackgroundTasks;
using CredentialsHandler;

public class TimedHostedService : BackgroundService
{
    private readonly ILogger<TimedHostedService> _logger;
    private readonly int _executionCount;
    private readonly IConfiguration _config;

    public TimedHostedService(ILogger<TimedHostedService> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");

        // When the timer should have no due-time, then do the work once now.
        DoWork();

        using PeriodicTimer timer = new(TimeSpan.FromMinutes(28));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                DoWork();
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");
        }
    }

    // Could also be a async method, that can be awaited in ExecuteAsync above
    private async void DoWork()
    {
        var cred = new CredHandler(_config);

        // await cred.Renew();
        
        // using StreamWriter outputFile = new("tokenlogs.txt", true);

        // await outputFile.WriteAsync("did work");

        Console.WriteLine("job ran");
    }
}