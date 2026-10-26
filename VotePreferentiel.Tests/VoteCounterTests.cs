using VotePreferentiel;

namespace VotePreferentiel.Tests;

public class VoteCounterTests
{
    [Test]
    public async Task GetWinner_ExampleVotes_ReturnsChloe()
    {
        // Arrange: the five ballots from the assignment.
        var voteModel = new VoteModel
        {
            Votes =
            [
                new Vote
                {
                    Choices = ["Alice", "Bilal", "Chloé"]
                },
                new Vote
                {
                    Choices = ["Alice", "Chloé", "Bilal"]
                },
                new Vote
                {
                    Choices = ["Bilal", "Chloé", "Alice"]
                },
                new Vote
                {
                    Choices = ["Chloé", "Bilal", "Alice"]
                },
                new Vote
                {
                    Choices = ["Chloé", "Alice", "Bilal"]
                }
            ]
        };

        var counter = new VoteCounter();

        // Act.
        var winner = counter.GetWinner(voteModel);

        // Assert.
        await Assert.That(winner).IsEqualTo("Chloé");
    }

    [Test]
    public async Task GetWinner_SingleCandidate_ReturnsAlice()
    {
        // Arrange.
        var voteModel = new VoteModel
        {
            Votes =
            [
                new Vote
            {
                Choices = ["Alice"]
            }
            ]
        };

        var counter = new VoteCounter();

        // Act.
        var winner = counter.GetWinner(voteModel);

        // Assert.
        await Assert.That(winner).IsEqualTo("Alice");
    }

    [Test]
    public async Task GetWinner_PreferenceTransfers_ReturnsChloe()
    {
        var voteModel = new VoteModel();

        for (var i = 0; i < 4; i++)
        {
            voteModel.Votes.Add(new Vote
            {
                Choices = ["Alice", "Bilal", "Chloé", "Diane"]
            });
        }

        for (var i = 0; i < 3; i++)
        {
            voteModel.Votes.Add(new Vote
            {
                Choices = ["Bilal", "Chloé", "Alice", "Diane"]
            });
        }

        for (var i = 0; i < 3; i++)
        {
            voteModel.Votes.Add(new Vote
            {
                Choices = ["Chloé", "Alice", "Bilal", "Diane"]
            });
        }

        voteModel.Votes.Add(new Vote
        {
            Choices = ["Diane", "Chloé", "Bilal", "Alice"]
        });

        var counter = new VoteCounter();

        var winner = counter.GetWinner(voteModel);

        await Assert.That(winner).IsEqualTo("Chloé");
    }

    [Test]
    public async Task GetWinner_TiedLastPlace_ReturnsChloe()
    {
        var voteModel = new VoteModel
        {
            Votes =
            [
                new Vote { Choices = ["Alice", "Chloé", "Bilal"] },
                new Vote { Choices = ["Bilal", "Chloé", "Alice"] },
                new Vote { Choices = ["Chloé", "Alice", "Bilal"] },
                new Vote { Choices = ["Chloé", "Bilal", "Alice"] }
            ]
        };

        var counter = new VoteCounter();

        var winner = counter.GetWinner(voteModel);

        await Assert.That(winner).IsEqualTo("Chloé");
    }

    [Test]
    public async Task GetWinner_EmptyBallot_ThrowsArgumentException()
    {
        var voteModel = new VoteModel
        {
            Votes =
            [
                new Vote { Choices = [] }
            ]
        };

        var counter = new VoteCounter();

        await Assert.That(() => counter.GetWinner(voteModel))
            .Throws<ArgumentException>();
    }

}