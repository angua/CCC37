using System.Collections.Generic;
using System.Numerics;
using System.Text;

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
                var tournament = new Tournament()
                {
                    FileNumber = inputFileNumber,
                    TournamentNumber = ++inputCount,
                    Level = level
                };


                if (level == 3 || level == 4 || level == 5)
                {
                    tournament.Set = ParseSet(input);
                    tournament.FigherCount = tournament.Set.Count;
                }

                else
                {
                    // Level 1, 2, 6, 7
                    // parse lineup
                    tournament.Set = CreateSet(input);
                    tournament.Lineup = input;
                    tournament.Rounds = CreateRounds(input);
                    tournament.FigherCount = input.Length;
                }

                if (level == 7)
                {
                    tournament.PossibleRounds = CreatePossibleRounds(input);
                }

                tournamentList.Add(tournament);

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
        set[Fighter.Available] = lineup.Count(c => c == 'X');
        set[Fighter.Unknown] = lineup.Count(c => c == 'Z');

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

        var set = new FighterSet();
        set[Fighter.Rock] = rocks;
        set[Fighter.Paper] = papers;
        set[Fighter.Scissors] = scissors;

        if (parts.Count() == 5)
        {
            var spocks = int.Parse(parts[3]);
            var lizards = int.Parse(parts[4]);
            set[Fighter.Lizard] = lizards;
            set[Fighter.Spock] = spocks;
        }

        return set;
    }

    public static string RunTournamentForRounds(string tournament, int numRounds)
    {
        var fighters = tournament;

        for (int round = 0; round < numRounds; round++)
        {
            var roundFighters = fighters.ToArray();
            var roundResult = new StringBuilder();

            for (int f = 0; f < roundFighters.Length; f += 2)
            {
                roundResult.Append(Extensions.GetOutcome(roundFighters[f], roundFighters[f + 1]));
            }

            fighters = roundResult.ToString();
        }

        return fighters;
    }


    public static string Solve(Tournament tournament)
    {
        switch (tournament.Level)
        {
            case 1:
                return SolveLevel1(tournament);
            case 2:
                return SolveLevel2(tournament);
            case 3:
                return SolveLevel3(tournament);
            case 4:
                return SolveLevel4(tournament);
            case 5:
                return SolveLevel5(tournament);
            case 6:
                return SolveLevel6(tournament);
            case 7:
                return SolveLevel7(tournament);
        };

        throw new InvalidOperationException("Cannot solve for tournament");

    }


    private static string SolveLevel1(Tournament tournament)
    {
        return tournament.Lineup.GetOutcome().ToString();
    }

    private static string SolveLevel2(Tournament tournament)
    {
        return RunTournamentForRounds(tournament.Lineup, 2);
    }

    private static string SolveLevel3(Tournament tournament)
    {
        List<string> pairs = new List<string>();

        var scissors = tournament.Set[Fighter.Scissors];
        var papers = tournament.Set[Fighter.Paper];
        var rocks = tournament.Set[Fighter.Rock];


        // Avoid pairing a single scissor with a rock
        if (scissors == 1 && papers > 0)
        {
            pairs.Add("SP");
            scissors--;
            papers--;
        }

        // Pair Rocks with Paper
        var rpPairs = Math.Min(rocks, papers);
        pairs.AddRange(Enumerable.Range(1, rpPairs).Select(_ => "RP"));
        rocks -= rpPairs;
        papers -= rpPairs;

        // Pair remaining Papers with Scissors
        var spPairs = Math.Min(papers, scissors);
        pairs.AddRange(Enumerable.Range(1, spPairs).Select(_ => "SP"));
        scissors -= spPairs;
        papers -= spPairs;

        var rrPairs = Math.DivRem(rocks, 2, out rocks);
        pairs.AddRange(Enumerable.Range(1, rrPairs).Select(_ => "RR"));

        if (rocks > 1) throw new InvalidOperationException("Too many rocks error");

        if (rocks == 1 && scissors > 0)
        {
            pairs.Add("RS");
            rocks--;
            scissors--;
        }

        if (rocks > 0) throw new InvalidOperationException("Too many rocks error");

        // Match remaining Papers with themselves
        var ppPairs = Math.DivRem(papers, 2, out papers);
        pairs.AddRange(Enumerable.Range(1, ppPairs).Select(_ => "PP"));

        // Match remaining scissors with themselves
        var ssPairs = Math.DivRem(scissors, 2, out scissors);
        pairs.AddRange(Enumerable.Range(1, ssPairs).Select(_ => "SS"));

        if (scissors > 0 || papers > 0 || rocks > 0) throw new InvalidOperationException("Logic error");

        pairs.Sort();

        for (int i = 0; i < pairs.Count; i++)
        {
            if (i == 0 || pairs[i - 1] == "RP")
            {
                if (TryFindPairToTheRight("RS", i, out var index))
                {
                    SwapPositions(i, index);
                    continue;
                }

                if (TryFindPairToTheRight("RR", i, out index))
                {
                    SwapPositions(i, index);
                    continue;
                }
            }
        }

        return string.Join("", pairs);

        bool TryFindPairToTheRight(string pair, int startIndex, out int index)
        {
            index = pairs.IndexOf(pair, startIndex);
            return index != -1;
        }

        void SwapPositions(int index1, int index2)
        {
            var temp = pairs[index1];
            pairs[index1] = pairs[index2];
            pairs[index2] = temp;
        }
    }

    private static string SolveLevel4(Tournament tournament)
    {
        var pairs = new Dictionary<string, int>();

        var scissors = tournament.Set[Fighter.Scissors];
        var papers = tournament.Set[Fighter.Paper];
        var rocks = tournament.Set[Fighter.Rock];

        pairs["RR"] = 0;
        pairs["RS"] = 0;
        pairs["RP"] = 0;
        pairs["SP"] = 0;
        pairs["PP"] = 0;
        pairs["SS"] = 0;

        // Avoid pairing a single scissor with a rock
        if (scissors == 1 && papers > 0)
        {
            pairs["SP"]++;
            scissors--;
            papers--;
        }

        // Pair Rocks with Paper
        var rpPairs = Math.Min(rocks, papers);
        pairs["RP"] += rpPairs;
        rocks -= rpPairs;
        papers -= rpPairs;

        // Pair remaining Papers with Scissors
        var spPairs = Math.Min(papers, scissors);
        pairs["SP"] += spPairs;
        scissors -= spPairs;
        papers -= spPairs;

        var rrPairs = Math.DivRem(rocks, 2, out rocks);
        pairs["RR"] += rrPairs;

        if (rocks > 1) throw new InvalidOperationException("Too many rocks error");

        if (rocks == 1 && scissors > 0)
        {
            pairs["RS"]++;
            rocks--;
            scissors--;
        }

        if (rocks > 0) throw new InvalidOperationException("Too many rocks error");

        // Match remaining Papers with themselves
        var ppPairs = Math.DivRem(papers, 2, out papers);
        pairs["PP"] += ppPairs;

        // Match remaining scissors with themselves
        var ssPairs = Math.DivRem(scissors, 2, out scissors);
        pairs["SS"] += ssPairs;

        if (scissors > 0 || papers > 0 || rocks > 0) throw new InvalidOperationException("Logic error");

        var pairCount = pairs.Sum(p => p.Value);

        var lineupList = new List<string>();

        lineupList.AddRange(Enumerable.Range(1, pairs["RS"]).Select(_ => "RS"));
        lineupList.AddRange(Enumerable.Range(1, pairs["RR"]).Select(_ => "RR"));
        lineupList.AddRange(Enumerable.Range(1, pairs["RP"]).Select(_ => "RP"));
        lineupList.AddRange(Enumerable.Range(1, pairs["PP"]).Select(_ => "PP"));
        lineupList.AddRange(Enumerable.Range(1, pairs["SP"]).Select(_ => "SP"));
        lineupList.AddRange(Enumerable.Range(1, pairs["SS"]).Select(_ => "SS"));

        var position = 0;
        var half = pairCount / 2;

        while (half > 0)
        {
            var rpIndex = lineupList.FindIndex(position, s => s == "RP");

            if (rpIndex != -1 && lineupList[position] != "RP")
            {
                SwapPositions(rpIndex, position);
            }

            position += half;
            half /= 2;
        }

        return string.Join("", lineupList);

        void SwapPositions(int index1, int index2)
        {
            var temp = lineupList[index1];
            lineupList[index1] = lineupList[index2];
            lineupList[index2] = temp;
        }
    }

    /// <summary>
    /// Level 5: Try arranging fighters for scissors to win
    /// </summary>
    /// <param name="tournament"></param>
    /// <returns></returns>
    public static string SolveLevel5(Tournament tournament)
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
        foreach (var fighter in StartFighters)
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
        if (leftAncestor == Fighter.Available && rightAncestor == Fighter.Available)
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
        else if (leftAncestor == Fighter.Available)
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


    private static string SolveLevel7(Tournament tournament)
    {
        // CreatePossibleRounds(tournament);

        return tournament.Lineup;
    }

    private static List<List<HashSet<char>>> CreatePossibleRounds(string lineup)
    {
        var possibleFighters = new List<HashSet<char>>();

        // allow multiple possible fighters for each position
        foreach (var fighter in lineup)
        {
            if (fighter == 'Z')
            {
                possibleFighters.Add(new HashSet<char>()
                {
                    'R','P','S','Y','L'
                });
            }
            else
            {
                possibleFighters.Add(new HashSet<char>()
                {
                    fighter
                });
            }

        }
        return CreateRounds(possibleFighters);
    }

    private static List<List<HashSet<char>>> CreateRounds(List<HashSet<char>> lineup)
    {
        var rounds = new List<List<HashSet<char>>>()
        {
            lineup
        };

        while (lineup.Count() > 1)
        {

            lineup = RunTournamentForRounds(lineup, 1);
            rounds.Add(lineup);
        }

        return rounds;

    }


    // allow multiple possible fighters for each position
    private static List<HashSet<char>> RunTournamentForRounds(List<HashSet<char>> lineup, int numRounds)
    {
        var result = new List<HashSet<char>>();
        for (int round = 0; round < numRounds; round++)
        {
            for (int position = 0; position < lineup.Count; position += 2)
            {
                var possibleFighters = new HashSet<char>();

                var fighterPos1 = lineup[position];
                var fighterPos2 = lineup[position + 1];

                foreach (var fighter1 in fighterPos1)
                {
                    foreach (var fighter2 in fighterPos2)
                    {
                        var pair = string.Join("", fighter1, fighter2);
                        possibleFighters.Add(Extensions.GetOutcome(fighter1, fighter2));
                    }
                }
                result.Add(possibleFighters);
            }

        }

        return result;
    }


    public static List<List<string>> CreateRounds(string lineup)
    {
        var results = new List<List<string>>
        {
            lineup.Select(f => f.ToString()).ToList()
        };

        while (lineup.Count() > 1)
        {
            lineup = RunTournamentForRounds(lineup, 1);
            results.Add(lineup.Select(f => f.ToString()).ToList());
        }

        return results;
    }

    public static Fighter GetFighter(string input)
    {
        return input switch
        {
            "R" => Fighter.Rock,
            "P" => Fighter.Paper,
            "S" => Fighter.Scissors,
            "L" => Fighter.Lizard,
            "Y" => Fighter.Spock,
            "X" => Fighter.Available,
            _ => throw new InvalidOperationException($"Unknown fighter {input}")
        };
    }


}
