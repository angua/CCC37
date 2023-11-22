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
        //Level1();
        //Level2();
        //Level3();
        //Level4();
        //Level5();
        Level6();

        Console.WriteLine("Done");
    }


    public void CreateOutput(int level)
    {
        // parse
        var tournaments = TournamentHandler.ParseTournaments(6);

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
            }
        }
    }


    private static void Level7()
    {

    }


    private static void Level6()
    {
        var tournaments = TournamentHandler.ParseTournaments(6);

        for (var inputFileNumber = 1; inputFileNumber <= 5; inputFileNumber++)
        {
            // parse
            var inputfilename = $"../../../../Files/level6_{inputFileNumber}.in";
            var outputfilename = $"../../../../Files/level6_{inputFileNumber}.out";

            Console.WriteLine($"input file {inputFileNumber}");

            var currentTournaments = tournaments.Where(t => t.FileNumber == inputFileNumber).ToList();

            using var outputWriter = new StreamWriter(outputfilename);

            for (var inputNumber = 1; inputNumber <= currentTournaments.Count; inputNumber++)
            {
                Console.WriteLine($"Input {inputNumber}");
                // 3R 11P 2S 15Y 1L

                var tournament = currentTournaments.First(t => t.TournamentNumber == inputNumber);

                var lineupString = TournamentHandler.SolveLevel6(tournament);

                outputWriter.WriteLine(lineupString);
                Console.WriteLine(lineupString);

                // (in)sanity check
                // check if scissors really win
                var tournamentResult = TournamentHandler.RunTournamentForRounds(lineupString, (int)Math.Log2(tournament.FigherCount), true);
                if (!tournamentResult.Contains('S')) throw new InvalidOperationException("No scissors left");
            }
        }
    }


    private static void Level5()
    {
        var tournaments = TournamentHandler.ParseTournaments(5);

        for (var inputFileNumber = 1; inputFileNumber <= 5; inputFileNumber++)
        {
            var outputfilename = $"../../../../Files/level5_{inputFileNumber}.out";
            using var outputWriter = new StreamWriter(outputfilename);

            var currentTournaments = tournaments.Where(t => t.FileNumber == inputFileNumber).ToList();
            Console.WriteLine($"input file {inputFileNumber}");

            for (var inputNumber = 1; inputNumber <= currentTournaments.Count; inputNumber++)
            {
                Console.WriteLine($"Input {inputNumber}");
                // 3R 11P 2S 15Y 1L

                var tournament = currentTournaments.First(t => t.TournamentNumber == inputNumber);

                var lineupString = TournamentHandler.SolveLevel5(tournament);

                outputWriter.WriteLine(lineupString);
                Console.WriteLine(lineupString);

                // (in)sanity check
                // check if scissors really win
                var tournamentResult = TournamentHandler.RunTournamentForRounds(lineupString, (int)Math.Log2(tournament.FigherCount), true);
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



    private static void Level4()
    {
        for (var inputFileNumber = 1; inputFileNumber <= 5; inputFileNumber++)
        {
            var inputfilename = $"../../../level4_{inputFileNumber}.in";
            var outputfilename = $"../../../level4_{inputFileNumber}.out";

            var lines = File.ReadAllLines(inputfilename).ToList();
            var tournaments = lines.Skip(1).ToList();

            using var outputWriter = new StreamWriter(outputfilename);

            foreach (var input in tournaments)
            {
                var line = input.Replace('R', ' ');
                line = line.Replace('P', ' ');
                line = line.Replace('S', ' ');

                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var rocks = int.Parse(parts[0]);
                var papers = int.Parse(parts[1]);
                var scissors = int.Parse(parts[2]);

                var pairs = new Dictionary<string, int>();

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

                Console.WriteLine(string.Join(" ", lineupList));

                var lineup = string.Join("", lineupList);
                outputWriter.WriteLine(lineup);

                var tournamentResult = TournamentHandler.RunTournamentForRounds(lineup, (int)Math.Log2(lineupList.Count));

                if (tournamentResult.Contains('R')) throw new InvalidOperationException("No rocks allowed here");
                if (!tournamentResult.Contains('S')) throw new InvalidOperationException("No scissors left");

                Console.WriteLine("After all rounds: {0}", tournamentResult);

                void SwapPositions(int index1, int index2)
                {
                    var temp = lineupList[index1];
                    lineupList[index1] = lineupList[index2];
                    lineupList[index2] = temp;
                }
            }
        }
    }

    private static void Level3()
    {
        for (var inputFileNumber = 1; inputFileNumber <= 5; inputFileNumber++)
        {
            var inputfilename = $"../../../level3_{inputFileNumber}.in";
            var outputfilename = $"../../../level3_{inputFileNumber}.out";

            var lines = File.ReadAllLines(inputfilename).ToList();
            var tournaments = lines.Skip(1).ToList();

            using var outputWriter = new StreamWriter(outputfilename);

            foreach (var input in tournaments)
            {
                var line = input.Replace('R', ' ');
                line = line.Replace('P', ' ');
                line = line.Replace('S', ' ');

                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var rocks = int.Parse(parts[0]);
                var papers = int.Parse(parts[1]);
                var scissors = int.Parse(parts[2]);

                List<string> pairs = new List<string>();

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

                Console.WriteLine(string.Join(" ", pairs));

                var lineup = string.Join("", pairs);
                outputWriter.WriteLine(lineup);

                var tournamentResult = TournamentHandler.RunTournamentForRounds(lineup, 2);

                if (tournamentResult.Contains('R')) throw new InvalidOperationException("No rocks allowed here");
                if (!tournamentResult.Contains('S')) throw new InvalidOperationException("No scissors left");

                Console.WriteLine("After two rounds: {0}", tournamentResult);

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
        }
    }

    private static void Level2()
    {
        for (var i = 1; i <= 5; i++)
        {
            var inputfilename = $"../../../level2_{i}.in";
            var outputfilename = $"../../../level2_{i}.out";

            var lines = File.ReadAllLines(inputfilename).ToList();
            var tournaments = lines.Skip(1).ToList();
            using var outputWriter = new StreamWriter(outputfilename);

            foreach (var tournament in tournaments)
            {
                var fighters = TournamentHandler.RunTournamentForRounds(tournament, 2);

                outputWriter.WriteLine(fighters);
            }
        }
    }


    private static void Level1()
    {

        for (var i = 1; i <= 5; i++)
        {
            var inputfilename = $"../../../level1_{i}.in";
            var outputfilename = $"../../../level1_{i}.out";

            var lines = File.ReadAllLines(inputfilename).ToList();
            var fights = lines.Skip(1).ToList();

            var result = new StringBuilder();

            //result.AppendLine(fights.Count.ToString());

            foreach (var fight in fights)
            {
                result.AppendLine(fight.GetOutCome().ToString());
            }

            File.WriteAllText(outputfilename, result.ToString());
        }
    }
}