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
                if (level == 5)
                {
                    // parse fighter set
                    var set = ParseSet(input);
                    tournamentList.Add(new Tournament()
                    {
                        FileNumber = inputFileNumber,
                        TournamentNumber = ++inputCount,
                        Set = set,
                        FigherCount = set.Count,
                        Level = level
                    });
                }
                else
                {
                    // Level 6
                    // parse lineup
                    tournamentList.Add(new Tournament()
                    {
                        FileNumber = inputFileNumber,
                        TournamentNumber = ++inputCount,
                        Set = CreateSet(input),
                        Lineup = input,
                        Rounds = CreateRounds(input),
                        FigherCount = input.Length,
                        Level = level
                    });

                }

            }

        }

        return tournamentList;
    }

    /// <summary>
    /// Create FighterSet from lineup
    /// </summary>
    /// <param name="lineup"></param>
    /// <returns></returns>
    private static FighterSet CreateSet(string lineup)
    {
        var set = new FighterSet();

        set[Fighter.Rock] = lineup.Count(c => c == 'R');
        set[Fighter.Paper] = lineup.Count(c => c == 'P');
        set[Fighter.Scissors] = lineup.Count(c => c == 'S');
        set[Fighter.Lizard] = lineup.Count(c => c == 'L');
        set[Fighter.Spock] = lineup.Count(c => c == 'Y');
        set[Fighter.Unknown] = lineup.Count(c => c == 'X');

        return set;
    }

    private static FighterSet ParseSet(string? input)
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

        return set;
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


    public static string Solve(Tournament tournament)
    {
        if (tournament.Level == 5)
        {
            return GuessSolution(tournament);
        }
        return SolveLevel6(tournament);
    }


    /// <summary>
    /// Level 5: Try arranging fighters for scissors to win
    /// </summary>
    /// <param name="tournament"></param>
    /// <returns></returns>
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
            else if (inputSet[Fighter.Lizard] > 0)
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


    public static string SolveLevel6(Tournament tournament)
    {
        // Start with placing Scissors at the winning round and work back up through the rounds trying differnt combinations

        // last round producing the winner
        var currentRound = tournament.Rounds.Count - 1;
        // lineup position of the winner
        var pos = 0;

        var winner = Fighter.Scissors;

        TryGetCombination(tournament, winner, pos, currentRound, out List<StartFighter> StartFighters);

        var result = tournament.Lineup.ToArray();
        foreach (var  fighter in StartFighters) 
        {
            result[fighter.Position] = fighter.StartFighterType.ToChar();
        }

        return string.Join("", result);
    }


    private static bool TryGetCombination(Tournament tournament, Fighter winner, int pos, int currentRound, out List<StartFighter> startFighters)
    {
        if (currentRound == 0)
        {
            // we have reached the top row
            // just position the winner here
            startFighters = new List<StartFighter>
            {
                new StartFighter()
                {
                    Position = pos,
                    StartFighterType = winner
                }
            };
            return true;
        }


        var possibleAncestorCombinations = new List<(Fighter, Fighter)>();

        startFighters = new List<StartFighter>();

        var inferiors = winner.GetInferiors();

        // get the "ancestors" of the current fighter
        var ancestorRound = currentRound - 1;

        var leftancestorPos = pos * 2;
        var leftAncestor = GetFighter(tournament.Rounds[ancestorRound][leftancestorPos]);

        var rightancestorPos = pos * 2 + 1;
        var rightAncestor = GetFighter(tournament.Rounds[ancestorRound][rightancestorPos]);


        // both ancestors unknown
        if (leftAncestor == Fighter.Unknown && rightAncestor == Fighter.Unknown)
        {
            foreach (var inferior in inferiors)
            {
                possibleAncestorCombinations.Add((winner, inferior));
                possibleAncestorCombinations.Add((inferior, winner));
            }

            foreach (var combination in possibleAncestorCombinations)
            {
                // go down both branches

                var leftfound = TryGetCombination(tournament, combination.Item1, leftancestorPos, ancestorRound, out var leftstartFighters);
                var rightfound = TryGetCombination(tournament, combination.Item2, rightancestorPos, ancestorRound, out var rightstartFighters);

                if (leftfound && rightfound)
                {
                    startFighters.AddRange(leftstartFighters);
                    startFighters.AddRange(rightstartFighters);
                    return true;
                }
            }
            return false;
        }

        // only left unknown
        else if (leftAncestor == Fighter.Unknown)
        {
            // go down left branch
            if (rightAncestor == winner)
            {
                foreach (var inferior in inferiors)
                {
                    if (TryGetCombination(tournament, inferior, leftancestorPos, ancestorRound, out var leftstartFighters))
                    {
                        startFighters.AddRange(leftstartFighters);
                        return true;
                    }
                }
                return false;
            }
            else if (inferiors.Contains(rightAncestor))
            {
                if (TryGetCombination(tournament, winner, leftancestorPos, ancestorRound, out var leftstartFighters))
                {
                    startFighters.AddRange(leftstartFighters);
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }

        }

        // only right unknown
        else
        {
            // go down right branch
            if (leftAncestor == winner)
            {
                foreach (var inferior in inferiors)
                {
                    if (TryGetCombination(tournament, inferior, rightancestorPos, ancestorRound, out var rightstartFighters))
                    {
                        startFighters.AddRange(rightstartFighters);
                        return true;
                    }
                }
                return false;
            }
            else if (inferiors.Contains(leftAncestor))
            {
                if (TryGetCombination(tournament, winner, rightancestorPos, ancestorRound, out var rightstartFighters))
                {
                    startFighters.AddRange(rightstartFighters);
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }

    }

    private static string FirstGuess(Tournament tournament)
    {
        // pairs of the first round
        var pairs = new List<string>();

        for (int i = 0; i < tournament.Lineup.Length / 2; i++)
        {
            var first = tournament.Lineup[2 * i];
            var second = tournament.Lineup[2 * i + 1];
            pairs.Add(string.Join("", first, second));
        }

        var lineup = new List<string>();
        foreach (var pair in pairs)
        {
            if (pair.Contains('X'))
            {
                // remove rocks and Spocks with paper
                if (pair.Contains('R') || pair.Contains('Y'))
                {
                    lineup.Add(pair.Replace('X', 'P'));
                }
                else if (pair == "XX")
                {
                    lineup.Add("PP");
                }
                else
                {
                    lineup.Add(pair.Replace('X', 'S'));
                }
            }
            else
            {
                lineup.Add(pair);
            }

        }

        return string.Join("", lineup);
    }

    public static List<string> CreateRounds(string lineup)
    {
        var results = new List<string>
        {
            lineup
        };

        while (lineup.Count() > 1)
        {
            lineup = RunTournamentForRounds(lineup, 1, true);
            results.Add(lineup);
        }

        return results;
    }

    public static Fighter GetFighter(char input)
    {
        return input switch
        {
            'R' => Fighter.Rock,
            'P' => Fighter.Paper,
            'S' => Fighter.Scissors,
            'L' => Fighter.Lizard,
            'Y' => Fighter.Spock,
            'X' => Fighter.Unknown,
            _ => throw new InvalidOperationException($"Unknown fighter {input}")
        };
    }


}
