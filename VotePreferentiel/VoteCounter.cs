namespace VotePreferentiel;

public sealed class VoteCounter : IVoteCounter
{
    public HashSet<string> GetCandidates(VoteModel voteModel)
    {
        var candidates = new HashSet<string>();

        foreach (var vote in voteModel.Votes)
        {
            foreach (var candidate in vote.Choices)
            {
                candidates.Add(candidate);
            }
        }

        return candidates;
    }

    public string GetWinner(VoteModel voteModel)
    {
        ArgumentNullException.ThrowIfNull(voteModel);

        if (voteModel.Votes is null || voteModel.Votes.Count == 0)
        {
            throw new ArgumentException(
                "The election must contain at least one ballot.",
                nameof(voteModel));
        }

        foreach (var vote in voteModel.Votes)
        {
            if (vote is null ||
                vote.Choices is null ||
                vote.Choices.Count == 0)
            {
                throw new ArgumentException(
                    "A ballot must contain at least one choice.",
                    nameof(voteModel));
            }
        }

        // Copy the ballots so the original data stays intact.
        var workingModel = new VoteModel();

        foreach (var vote in voteModel.Votes)
        {
            workingModel.Votes.Add(new Vote
            {
                Choices = new List<string>(vote.Choices)
            });
        }

        var candidates = GetCandidates(workingModel);

        while (candidates.Count > 1)
        {
            var firstChoices = GetAllFirstChoices(workingModel);
            var counts = CountFirstChoices(firstChoices);

            // Candidates without first-choice votes still need a count.
            foreach (var candidate in candidates)
            {
                if (!counts.ContainsKey(candidate))
                {
                    counts[candidate] = 0;
                }
            }

            var eliminated = GetCandidatesWithLeastVotes(counts);

            if (eliminated.Length == candidates.Count)
            {
                throw new InvalidOperationException(
                    "All remaining candidates are tied. " +
                    "No unique winner can be determined.");
            }

            // Eliminate every tied loser before recounting.
            foreach (var candidate in eliminated)
            {
                candidates.Remove(candidate);

                foreach (var vote in workingModel.Votes)
                {
                    vote.Choices.RemoveAll(
                        choice => choice == candidate);
                }
            }
        }

        return candidates.Single();
    }

    public List<string> GetAllFirstChoices(VoteModel voteModel)
    {
        var firstChoices = new List<string>();

        foreach (var vote in voteModel.Votes)
        {
            if (vote.Choices.Count > 0)
            {
                firstChoices.Add(vote.Choices[0]);
            }
        }

        return firstChoices;
    }

    public IDictionary<string, int> CountFirstChoices(
    List<string> firstChoices)
    {
        var counts = new Dictionary<string, int>();

        foreach (var candidate in firstChoices)
        {
            if (counts.ContainsKey(candidate))
            {
                counts[candidate]++;
            }
            else
            {
                counts[candidate] = 1;
            }
        }

        return counts;
    }

    public string[] GetCandidatesWithLeastVotes(
    IDictionary<string, int> firstChoiceCounts)
    {
        if (firstChoiceCounts.Count == 0)
        {
            throw new ArgumentException(
                "The vote counts cannot be empty.",
                nameof(firstChoiceCounts));
        }

        var minimum = firstChoiceCounts.Values.Min();

        var candidates = new List<string>();

        foreach (var entry in firstChoiceCounts)
        {
            if (entry.Value == minimum)
            {
                candidates.Add(entry.Key);
            }
        }

        return candidates.ToArray();
    }
}