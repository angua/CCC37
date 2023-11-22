using RockPaperScissors;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace RockPaperScissors;

public class Program
{
    public static void Main(string[] args)
    {
        CreateOutput(7);

        Console.WriteLine("Done");
    }


    public static void CreateOutput(int level)
    {
        // parse
        var tournaments = TournamentHandler.ParseTournaments(level);

        for (var inputFileNumber = 1; inputFileNumber <= 5; inputFileNumber++)
        {
            var currentTournaments = tournaments.Where(t => t.FileNumber == inputFileNumber).ToList();
            Console.WriteLine($"input file {inputFileNumber}");

            var outputfilename = $"../../../../Files/level{level}_{inputFileNumber}.out";
            using var outputWriter = new StreamWriter(outputfilename);

            for (var inputNumber = 1; inputNumber <= currentTournaments.Count; inputNumber++)
            {
                var tournament = currentTournaments.First(t => t.TournamentNumber == inputNumber);
                Console.WriteLine($"Input {inputNumber}");

                var lineupString = TournamentHandler.Solve(tournament);

                outputWriter.WriteLine(lineupString);
                Console.WriteLine(lineupString);

                TestOutput(lineupString, tournament);
            }
        }
    }

    private static void TestOutput(string lineupString, Tournament tournament)
    {
        if (tournament.Level == 3)
        {
            var tournamentResult = TournamentHandler.RunTournamentForRounds(lineupString, 2);

            if (tournamentResult.Contains('R')) throw new InvalidOperationException("No rocks allowed here");
            if (!tournamentResult.Contains('S')) throw new InvalidOperationException("No scissors left");
        }
        else if (tournament.Level == 4)
        {
            var tournamentResult = TournamentHandler.RunTournamentForRounds(lineupString, (int)Math.Log2(lineupString.Length));

            if (tournamentResult.Contains('R')) throw new InvalidOperationException("No rocks allowed here");
            if (!tournamentResult.Contains('S')) throw new InvalidOperationException("No scissors left");
        }
        else if (tournament.Level == 5)
        {
            // (in)sanity check
            // check if scissors really win
            var tournamentResult = TournamentHandler.RunTournamentForRounds(lineupString, (int)Math.Log2(tournament.FigherCount));
            if (!tournamentResult.Contains('S')) throw new InvalidOperationException("No scissors left");

            if (lineupString.Count(c => c == 'S') != tournament.Set[Fighter.Scissors])
            {
                throw new InvalidOperationException("Incorrect number of scissors");
            }
            if (lineupString.Count(c => c == 'R') != tournament.Set[Fighter.Rock])
            {
                throw new InvalidOperationException("Incorrect number of rocks");
            }
            if (lineupString.Count(c => c == 'P') != tournament.Set[Fighter.Paper])
            {
                throw new InvalidOperationException("Incorrect number of papers");
            }
            if (lineupString.Count(c => c == 'L') != tournament.Set[Fighter.Lizard])
            {
                throw new InvalidOperationException("Incorrect number of lizards");
            }
            if (lineupString.Count(c => c == 'Y') != tournament.Set[Fighter.Spock])
            {
                throw new InvalidOperationException("Incorrect number of Spocks");
            }
        }
        else if (tournament.Level == 6)
        {
            // (in)sanity check
            // check if scissors really win
            var tournamentResult = TournamentHandler.RunTournamentForRounds(lineupString, (int)Math.Log2(tournament.FigherCount));
            if (!tournamentResult.Contains('S')) throw new InvalidOperationException("No scissors left");
        }
    }


    private static IEnumerable<(FighterSet RemainingFighters, Fighter[] Lineup)> GenerateFighterSet(Fighter[] winners, FighterSet inputSet, int numFighters)
    {
        Console.WriteLine($"num fighters {numFighters}, winners:{string.Join(' ', winners)} inputset: {inputSet.ToString()}");
        foreach (var winner in winners)
        {
            if (!inputSet.Contains(winner))
            {
                continue;
            }

            if (numFighters == 2)
            {
                // End of the recursion, produce something if we can

                // Reduce inputset by one winner
                var reducedInputSet = inputSet.Clone();
                reducedInputSet[winner]--;

                foreach (var inferior in winner.GetInferiors())
                {
                    if (reducedInputSet.Contains(inferior))
                    {
                        var possibility = reducedInputSet.Clone();
                        possibility[inferior]--;
                        Console.WriteLine($"found possiblity {winner} {inferior}");
                        yield return (RemainingFighters: possibility, Lineup: new[] { winner, inferior });
                    }
                }

                yield break;
            }

            // Spock-Strategy
            // remove rocks using Spocks then get rid of Spock with paper or lizard
            // sorry Mr. Spock, don't live long and prosper
            // LY YR YRRR YRRRRRRR ...
            if ((winner == Fighter.Lizard || winner == Fighter.Paper) &&
                inputSet[Fighter.Spock] >= (int)Math.Log2(numFighters) &&
                inputSet[Fighter.Spock] + inputSet[Fighter.Rock] >= numFighters - 1)
            {
                var lineup = new Fighter[numFighters];
                var possibility = inputSet.Clone();

                lineup[0] = winner;
                possibility[winner]--;
                lineup[1] = Fighter.Spock;
                possibility[Fighter.Spock]--;

                for (int i = 2; i < numFighters; i++)
                {
                    if (BitOperations.IsPow2(i))
                    {
                        lineup[i] = Fighter.Spock;
                        possibility[Fighter.Spock]--;
                    }
                    else if (possibility[Fighter.Rock] > 0)
                    {
                        lineup[i] = Fighter.Rock;
                        possibility[Fighter.Rock]--;
                    }
                    else
                    {
                        lineup[i] = Fighter.Spock;
                        possibility[Fighter.Spock]--;
                    }
                }

                yield return (RemainingFighters: possibility, Lineup: lineup);
            }

            // Paper Rocks strategy
            // PRRRRRRRR
            if (winner == Fighter.Paper && inputSet[Fighter.Rock] >= numFighters - 1)
            {
                var lineup = new Fighter[numFighters];
                var possibility = inputSet.Clone();

                lineup[0] = winner;
                possibility[winner]--;

                for (int i = 1; i < numFighters; i++)
                {
                    lineup[i] = Fighter.Rock;
                }
                possibility[Fighter.Rock] -= numFighters - 1;

                yield return (RemainingFighters: possibility, Lineup: lineup);
            }


            // split in halves
            var half = numFighters >> 1;

            // Reduce inputset for loser side by one winner
            var loserInputSet = inputSet.Clone();
            loserInputSet[winner]--;

            // The right half needs to produce somebody the winner can crush 
            foreach (var rightHalf in GenerateFighterSet(winner.GetInferiors(), loserInputSet, half))
            {
                // add winner back to winning side
                rightHalf.RemainingFighters[winner]++;

                // Combine this set with the left half who needs to produce the wanted winner
                foreach (var leftHalf in GenerateFighterSet(new[] { winner }, rightHalf.RemainingFighters, half))
                {
                    // Combine left half lineup with the right half
                    var lineup = new Fighter[numFighters];

                    // Copy the left half lineup to the left half of the resulting lineup
                    Array.Copy(leftHalf.Lineup, 0, lineup, 0, leftHalf.Lineup.Length);
                    // Copy the right half lineup to the right half of the resulting lineup
                    Array.Copy(rightHalf.Lineup, 0, lineup, half, rightHalf.Lineup.Length);

                    yield return (RemainingFighters: leftHalf.RemainingFighters, Lineup: lineup);
                }
            }
        }
    }


}