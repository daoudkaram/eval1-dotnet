namespace VotePreferentiel;

public interface IVoteCounter
{
    // Collect all candidates appearing in the ballots.
    HashSet<string> GetCandidates(VoteModel voteModel);

    // Determine the election winner.
    string GetWinner(VoteModel voteModel);

    // Get each voter's first choice.
    List<string> GetAllFirstChoices(VoteModel voteModel);

    // Count the first-choice votes for each candidate.
    IDictionary<string, int> CountFirstChoices(
        List<string> firstChoices);

    // Find all candidates tied for the lowest vote count.
    string[] GetCandidatesWithLeastVotes(
        IDictionary<string, int> firstChoiceCounts);
}