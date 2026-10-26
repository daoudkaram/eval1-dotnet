using System.Net.Http.Json;

namespace VotePreferentiel;

public sealed class VoteProvider
{
    private readonly HttpClient _httpClient;

    public VoteProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<VoteModel> GetVotesAsync(
        string url,
        CancellationToken cancellationToken = default)
    {
        var voteModel = await _httpClient.GetFromJsonAsync<VoteModel>(
            url,
            cancellationToken);

        return voteModel
            ?? throw new InvalidOperationException(
                "The server returned a null vote model.");
    }
}