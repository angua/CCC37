using System;
using System.Text;
using System.Xml;
using RockPaperScissors;

namespace Contest;

public class Program
{
    public static void Main(string[] args)
    {
        //Level1();
        //Level2();
        //Level3();
        //Level4();
        Level5();

        Console.WriteLine("Done");
    }
    private static void Level5()
    {
        for (var inputFileNumber = 1; inputFileNumber <= 5; inputFileNumber++)
        {
            var inputfilename = $"../../../level5_{inputFileNumber}.in";
            var outputfilename = $"../../../level5_{inputFileNumber}.out";

            var lines = File.ReadAllLines(inputfilename).ToList();
            var tournaments = lines.Skip(1).ToList();

            using var outputWriter = new StreamWriter(outputfilename);

            foreach (var input in tournaments)
            {
                // 3R 11P 2S 15Y 1L

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

                var fighterCount = rocks + papers + scissors + spocks + lizards;

                var lineupList = new Dictionary<int, string>();


                // left half
                if (papers > 0)
                {
                    // paper to remove rocks from the left
                    lineupList[0] = "P";
                    papers--;
                }
                else
                {
                    // spock to remove rocks from the left
                    lineupList[0] = "Y";
                    spocks--;
                }


                var firstEmptyPos = 1;

                // fill with rocks until the middle - 1 or out of rocks
                for (var pos = 1; pos < (int)fighterCount / 2 - 1; pos++)
                {
                    if (rocks > 0)
                    {
                        lineupList[pos] = "R";
                        rocks--;
                        firstEmptyPos = pos + 1;
                    }
                    else
                    {
                        firstEmptyPos = pos;
                        break;
                    }
                }

                // if more than 50% scissors, place them here
                if (scissors > fighterCount / 2)
                {

                }




                Console.WriteLine(string.Join(" ", lineupList));

                var lineup = string.Join("", lineupList);
                outputWriter.WriteLine(lineup);

                var tournamentResult = RunTournamentForRounds(lineup, (int)Math.Log2(lineupList.Count), true);

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

                var tournamentResult = RunTournamentForRounds(lineup, (int)Math.Log2(lineupList.Count));

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

                var tournamentResult = RunTournamentForRounds(lineup, 2);

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
                var fighters = RunTournamentForRounds(tournament, 2);

                outputWriter.WriteLine(fighters);
            }
        }
    }

    private static string RunTournamentForRounds(string tournament, int numRounds) => RunTournamentForRounds(tournament, numRounds, false);

    private static string RunTournamentForRounds(string tournament, int numRounds, bool use5Styles)
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