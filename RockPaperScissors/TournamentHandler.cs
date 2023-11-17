using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
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
        var inputSet = tournament.Set.Clone();

        // first positions get scissors
        var powerOf2Scissors = (int)Math.Log2(inputSet[Fighter.Scissors]);
        var posAfterScissors = (int)Math.Pow(2, powerOf2Scissors);

        for (int i = 0; i < posAfterScissors; i++)
        {
            lineup[i] = Fighter.Scissors;
            inputSet[Fighter.Scissors]--;
        }

        // second half (losers)
        var half = tournament.FigherCount / 2;

        while (inputSet.Count > 0)
        {
            FillSecondHalf(lineup, inputSet, half);
            half = half >> 1;
        }


        return new string(lineup.Select(f => f.ToChar()).ToArray());

    }

    private static void FillSecondHalf(Fighter[] lineup, FighterSet inputSet, int half)
    {
        // Paper Rocks strategy
        // PRRRRRRRR
        if (inputSet.Count > 1 && inputSet[Fighter.Paper] > 0 && inputSet[Fighter.Rock] >= half - 1)
        {
            lineup[half] = Fighter.Paper;
            inputSet[Fighter.Paper]--;

            for (int i = 1; i < half; i++)
            {
                lineup[half + i] = Fighter.Rock;
            }
            inputSet[Fighter.Rock] -= half - 1;

        }
        // Spock-Strategy
        // remove rocks using Spocks then get rid of Spock with paper or lizard
        // sorry Mr. Spock, don't live long and prosper
        // LY YR YRRR YRRRRRRR ...
        else if (inputSet.Count > 1 &&
            (inputSet[Fighter.Lizard] > 0 || inputSet[Fighter.Paper] > 0) &&
            inputSet[Fighter.Spock] >= (int)Math.Log2(half) &&
            inputSet[Fighter.Spock] + inputSet[Fighter.Rock] >= half - 1)
        {
            if (inputSet[Fighter.Lizard] > 0)
            {
                lineup[half] = Fighter.Lizard;
                inputSet[Fighter.Lizard]--;
            }
            else
            {
                lineup[half] = Fighter.Paper;
                inputSet[Fighter.Paper]--;
            }

            lineup[half + 1] = Fighter.Spock;
            inputSet[Fighter.Spock]--;

            for (int i = 2; i < half; i++)
            {
                if (BitOperations.IsPow2(i))
                {
                    lineup[half + i] = Fighter.Spock;
                    inputSet[Fighter.Spock]--;
                }
                else if (inputSet[Fighter.Rock] > 0)
                {
                    lineup[half + i] = Fighter.Rock;
                    inputSet[Fighter.Rock]--;
                }
                else
                {
                    lineup[half + i] = Fighter.Spock;
                    inputSet[Fighter.Spock]--;
                }
            }

        }
        
        else
        {
            // try placing lizard or paper at the first position
            
             if (inputSet[Fighter.Paper] > 0)
            {
                lineup[half] = Fighter.Paper;
                inputSet[Fighter.Paper]--;
            }
            else if(inputSet[Fighter.Lizard] > 0)
            {
                lineup[half] = Fighter.Lizard;
                inputSet[Fighter.Lizard]--;
            }
            else if (inputSet[Fighter.Spock] > 0)
            {
                lineup[half] = Fighter.Spock;
                inputSet[Fighter.Spock]--;
            }
            else if (inputSet[Fighter.Rock] > 0)
            {
                lineup[half] = Fighter.Rock;
                inputSet[Fighter.Rock]--;
            }
            else if (inputSet[Fighter.Scissors] > 0)
            {
                lineup[half] = Fighter.Scissors;
                inputSet[Fighter.Scissors]--;
            }

            // just fill the rest with whatever we have
            for (int i = 1; i < half; i++)
            {
                if (inputSet[Fighter.Spock] > 0)
                {
                    lineup[half + i] = Fighter.Spock;
                    inputSet[Fighter.Spock]--;
                }
                else if (inputSet[Fighter.Rock] > 0)
                {
                    lineup[half + i] = Fighter.Rock;
                    inputSet[Fighter.Rock]--;
                }
                else if (inputSet[Fighter.Scissors] > 0)
                {
                    lineup[half + i] = Fighter.Scissors;
                    inputSet[Fighter.Scissors]--;
                }
                else if (inputSet[Fighter.Lizard] > 0)
                {
                    lineup[half + i] = Fighter.Lizard;
                    inputSet[Fighter.Lizard]--;
                }
                else if (inputSet[Fighter.Paper] > 0)
                {
                    lineup[half + i] = Fighter.Paper;
                    inputSet[Fighter.Paper]--;
                }
            }
        }
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
