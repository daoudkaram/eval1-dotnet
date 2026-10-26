using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VotePreferentiel;

namespace VotePreferentiel.Worker;

public sealed class Worker : BackgroundService
{
    private readonly VoteProvider _voteProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<Worker> _logger;
    private readonly IHostApplicationLifetime _lifetime;

    public Worker(
        VoteProvider voteProvider,
        IConfiguration configuration,
        ILogger<Worker> logger,
        IHostApplicationLifetime lifetime)
    {
        _voteProvider = voteProvider;
        _configuration = configuration;
        _logger = logger;
        _lifetime = lifetime;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        try
        {
            var url = _configuration["Votes:Url"];

            if (string.IsNullOrWhiteSpace(url))
            {
                throw new InvalidOperationException(
                    "Missing configuration: Votes:Url.");
            }

            var voteModel = await _voteProvider.GetVotesAsync(
                url,
                stoppingToken);

            _logger.LogInformation(
                "Loaded {Count} ballots.",
                voteModel.Votes.Count);

            for (var i = 0; i < voteModel.Votes.Count; i++)
            {
                _logger.LogInformation(
                    "Ballot {Number}: {Choices}",
                    i + 1,
                    string.Join(" > ", voteModel.Votes[i].Choices));
            }
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            // Normal cancellation during application shutdown.
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to load the votes.");
            Environment.ExitCode = 1;
        }
        finally
        {
            _lifetime.StopApplication();
        }
    }
}