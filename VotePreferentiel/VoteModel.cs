namespace VotePreferentiel;

public sealed class VoteModel
{
    public List<Vote> Votes { get; set; } = [];
}

public sealed class Vote
{
    public List<string> Choices { get; set; } = [];
}