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

    private readonly IVoteCounter _voteCounter;

    public Worker(
        VoteProvider voteProvider,
        IVoteCounter voteCounter,
        IConfiguration configuration,
        ILogger<Worker> logger,
        IHostApplicationLifetime lifetime)
    {
        _voteProvider = voteProvider;
        _voteCounter = voteCounter;
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

            // to check if everyhting working like it should
            //for (var i = 0; i < voteModel.Votes.Count; i++)
            //{
            //    _logger.LogInformation(
            //        "Ballot {Number}: {Choices}",
            //        i + 1,
            //        string.Join(" > ", voteModel.Votes[i].Choices));
            //}

            var winner = _voteCounter.GetWinner(voteModel);
            _logger.LogInformation("The winner is: {Winner}", winner);
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            // Normal cancellation during application shutdown.
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to process the election.");
            Environment.ExitCode = 1;
        }
        finally
        {
            _lifetime.StopApplication();
        }
    }
}