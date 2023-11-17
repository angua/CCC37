using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockPaperScissors;

public static class TournamentHandler
{
    public static List<Tournament> ParseTournaments(int level)
    {
        var tournamentList = new List<Tournament>();

        for (var inputFileNumber = 1; inputFileNumber <= 5; inputFileNumber++)
        {
            var inputfilename = $"../../../../Files/level{level}_{inputFileNumber}.in";

            var lines = File.ReadAllLines(inputfilename).ToList();
            var tournaments = lines.Skip(1).ToList();

            var inputCount = 0;

            foreach (var input in tournaments)
            {
                var line = input.Replace('R', ' ');
                line = line.Replace('P', ' ');
                line = line.Replace('S', ' ');
                line = line.Replace('Y', ' ');
                line = line.Replace('L', ' ');

                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var rocks = int.Parse(parts[0]);
                var papers = int.Parse(parts[1]);
                var scissors = int.Parse(parts[2]);
                var spocks = int.Parse(parts[3]);
                var lizards = int.Parse(parts[4]);

                var set = new FighterSet();
                set[Fighter.Rock] = rocks;
                set[Fighter.Paper] = papers;
                set[Fighter.Scissors] = scissors;
                set[Fighter.Lizard] = lizards;
                set[Fighter.Spock] = spocks;

                tournamentList.Add(new Tournament()
                {
                    FileNumber = inputFileNumber,
                    TournamentNumber = ++inputCount,
                    Set = set,
                    FigherCount = rocks + papers + scissors + spocks + lizards
                });

            }

        }

        return tournamentList;
    }

    public static string RunTournamentForRounds(string tournament, int numRounds) => RunTournamentForRounds(tournament, numRounds, false);

    public static string RunTournamentForRounds(string tournament, int numRounds, bool use5Styles)
    {
        var fighters = tournament;

        for (int round = 0; round < numRounds; round++)
        {
            var roundFighters = fighters.ToArray();
            var roundResult = new StringBuilder();

            for (int f = 0; f < roundFighters.Length; f += 2)
            {
                if (!use5Styles)
                {
                    roundResult.Append(Extensions.GetOutCome(roundFighters[f], roundFighters[f + 1]));
                }
                else
                {
                    roundResult.Append(Extensions.Get5StylesOutCome(roundFighters[f], roundFighters[f + 1]));
                }
            }

            fighters = roundResult.ToString();
        }

        return fighters;
    }


    public static string GuessSolution(Tournament tournament)
    {
        var lineup = new Fighter[tournament.FigherCount];

        var reducedInputSet = tournament.Set.Clone();

        for (int i = 0; i < tournament.FigherCount; i++)
        {
            if (reducedInputSet[Fighter.Scissors] > 0)
            {
                lineup[i] = Fighter.Scissors;
                reducedInputSet[Fighter.Scissors]--;
            }
            else if (reducedInputSet[Fighter.Paper] > 0)
            {
                lineup[i] = Fighter.Paper;
                reducedInputSet[Fighter.Paper]--;
            }
            else if (reducedInputSet[Fighter.Lizard] > 0)
            {
                lineup[i] = Fighter.Lizard;
                reducedInputSet[Fighter.Lizard]--;
            }

            else if (reducedInputSet[Fighter.Spock] > 0)
            {
                lineup[i] = Fighter.Spock;
                reducedInputSet[Fighter.Spock]--;
            }
            else if (reducedInputSet[Fighter.Rock] > 0)
            {
                lineup[i] = Fighter.Rock;
                reducedInputSet[Fighter.Rock]--;
            }

        }

        return new string(lineup.Select(f => f.ToChar()).ToArray());

    }

    public static List<string> CreateRounds(string result)
    {
        var results = new List<string>
        {
            result
        };

        while (result.Count() > 1)
        {
            result = RunTournamentForRounds(result, 1, true);
            results.Add(result);
        }

        return results;
    }


}
