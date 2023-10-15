using RockPaperScissors;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace Contest;

public partial class Program
{
    public class FighterSet
    {
        private int[] _fighters = new int[5];

        public int this[Fighter fighter]
        {
            get => _fighters[(int)fighter];
            set => _fighters[(int)fighter] = value;
        }

        public bool Contains(Fighter fighter) => this[fighter] > 0;

        public FighterSet Clone()
        {
            var clone = new FighterSet();
            Array.Copy(_fighters, clone._fighters, _fighters.Length);
            return clone;
        }

        internal bool IsEmpty()
        {
            return _fighters.Sum() == 0;
        }

        internal void ValidateLineup(Fighter[] lineup)
        {
            foreach (Fighter fighter in Enum.GetValues(typeof(Fighter)))
            {
                if (lineup.Count(f => f == fighter) != this[fighter])
                {
                    throw new InvalidOperationException($"Fighter type not used: {fighter}");
                }
            }
        }
    }

    public static void Main(string[] args)
    {
        //Level1();
        //Level2();
        //Level3();
        //Level4();
        Level5();

        Console.WriteLine("Done");
    }

    private class SubResult
    {
        public SubResult()
        {
            RemainingFighters = new();
            Lineup = new Fighter[0];
        }

        public SubResult(FighterSet remainingFighters, Fighter[] lineup)
        {
            RemainingFighters = remainingFighters;
            Lineup = lineup;
        }

        public FighterSet RemainingFighters { get; set; }
        public Fighter[] Lineup { get; set; }
    }

    private static void Level5()
    {
        //var testinput = "98R 4P 10S 16Y 0L";
        //var testResult = ProcessTournament(testinput);


        for (var inputFileNumber = 1; inputFileNumber <= 5; inputFileNumber++)
        {
            var inputfilename = $"../../../level5_{inputFileNumber}.in";
            var outputfilename = $"../../../level5_{inputFileNumber}.out";

            var lines = File.ReadAllLines(inputfilename).ToList();
            var tournaments = lines.Skip(1).Select((tournament, idx) => (Tournament: tournament, Index: idx)).ToList();

            var stopwatch = new Stopwatch();

            using var outputWriter = new StreamWriter(outputfilename);

            stopwatch.Start();

            var results = new List<SubResult>(Enumerable.Range(1, tournaments.Count).Select(_ => new SubResult()));

            var partitioner = Partitioner.Create(tournaments, true);

            partitioner.AsParallel().WithDegreeOfParallelism(8).ForAll(indexedTournament =>
            {
                var input = indexedTournament.Tournament;

                Console.WriteLine("[File {2}] Thread {3} Processing input: {0}/{1} {4}",
                    indexedTournament.Index, tournaments.Count, inputFileNumber, Thread.CurrentThread.ManagedThreadId, indexedTournament.Tournament);

                var result = ProcessTournament(input);

                //Console.WriteLine("After all rounds: {0}", tournamentResult);
                results[indexedTournament.Index] = result;

                Console.WriteLine("[File {2}] Thread {3} Input: {0}/{1} DONE",
                    indexedTournament.Index, tournaments.Count, inputFileNumber, Thread.CurrentThread.ManagedThreadId);
            });

            stopwatch.Stop();

            // Write all results to the file, in order
            foreach (var result in results)
            {
                var lineupString = result.Lineup.ToTournament();

                var tournamentResult = RunTournamentForRounds(lineupString, (int)Math.Log2(lineupString.Length), true);

                if (tournamentResult.Contains('R')) throw new InvalidOperationException("No rocks allowed here");
                if (!tournamentResult.Contains('S')) throw new InvalidOperationException("No scissors left");

                outputWriter.WriteLine(lineupString);
            }

            Console.WriteLine("\n-------- [{0:0} ms]", stopwatch.ElapsedMilliseconds);
        }
    }

    private static SubResult ProcessTournament(string input)
    {
        // 3R 11P 2S 15Y 1L
        //Console.SetCursorPosition(0, Console.GetCursorPosition().Top);
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

        var fighterCount = rocks + papers + scissors + spocks + lizards;

        var lineup = new Fighter[fighterCount];

        var result = GenerateFighterSet(new[] { Fighter.Scissors }, set, lineup.Length).First();

        if (!result.RemainingFighters.IsEmpty()) throw new InvalidOperationException("Not all the fighters have been assigned");

        set.ValidateLineup(result.Lineup);

        return result;
    }

    private static IEnumerable<SubResult> GenerateFighterSet(Fighter[] winners, FighterSet inputSet, int numFighters)
    {
        if (numFighters == 2)
        {
            // End of the recursion, produce something if we can
            foreach (var winner in winners)
            {
                if (inputSet.Contains(winner))
                {
                    // Reduce inputset by one winner
                    var reducedInputSet = inputSet.Clone();
                    reducedInputSet[winner]--;

                    foreach (var inferior in winner.GetInferiors())
                    {
                        if (reducedInputSet.Contains(inferior))
                        {
                            var remaining = reducedInputSet.Clone();
                            remaining[inferior]--;
                            yield return new SubResult(remaining, new[] { inferior, winner });
                        }
                    }
                }
            }

            yield break;
        }

        var half = numFighters >> 1;

        foreach (var winner in winners)
        {
            if (!inputSet.Contains(winner)) continue;

#if false // Shortcuts
            var inferiors = winner.GetInferiors().ToList();

            var inferiorSum = inferiors.Select(i => inputSet[i]).Sum();

            if (inferiorSum >= numFighters) // inferiors include the winner itself
            {
                // Bunch up inferiors into this tree
                var newSet = inputSet.Clone();
                var lineup = new Fighter[numFighters];

                var inferiorsToPlace = numFighters - 1;
                var insertPosition = 0;
                while (inferiorsToPlace > 0 && inferiors.Count > 0)
                {
                    var inferior = inferiors[0]; // take the next best inferior
                    inferiors.Remove(inferior);

                    // Take as much as we need, but not more than we have
                    var inferiorsToTake = Math.Min(newSet[inferior], inferiorsToPlace);

                    // Don't use up all winners
                    if (inferior == winner && newSet[inferior] - inferiorsToTake == 0)
                    {
                        inferiorsToTake--;
                    }

                    Array.Fill(lineup, inferior, insertPosition, inferiorsToTake);

                    insertPosition += inferiorsToTake;
                    inferiorsToPlace -= inferiorsToTake;
                    newSet[inferior] -= inferiorsToTake;
                }

                if (insertPosition != numFighters - 1) throw new InvalidOperationException("Insertion gone wrong");

                // Place the desired winner in one corner
                lineup[lineup.Length - 1] = winner;
                newSet[winner]--;

                //Console.WriteLine("Yielding Shortcut {0}", lineup.ToTournament());

                yield return new SubResult { RemainingFighters = newSet, Lineup = lineup };
            } 
#endif

#if false
            if (numFighters > 16)
            {
                // Check if we enough inferiors to just fill up the whole set
                foreach (var inferior in winner.GetInferiors())
                {
                    if (winner != inferior && inputSet[inferior] >= numFighters - 1 && inputSet[winner] > 0 ||
                        winner == inferior && inputSet[winner] >= numFighters)
                    {
                        var newSet = inputSet.Clone();
                        newSet[inferior] -= numFighters - 1;
                        newSet[winner]--;

                        var lineup = new Fighter[numFighters];
                        Array.Fill(lineup, inferior, 0, numFighters - 1);
                        lineup[lineup.Length - 1] = winner;

                        yield return (RemainingFighters: newSet, Lineup: lineup);
                    }
                }
            } 
#endif

            // Create combinations
            // The right half needs to produce the wanted winner
            foreach (var rightHalf in GenerateFighterSet(new[] { winner }, inputSet, half))
            {
                // Combine this set with the left half who needs to produce somebody the winner can crush
                foreach (var leftHalf in GenerateFighterSet(winner.GetInferiors(), rightHalf.RemainingFighters, half))
                {
                    // Combine left half lineup with the right half
                    var lineup = new Fighter[numFighters];

                    // Copy the left half lineup to the left half of the resulting lineup
                    Array.Copy(leftHalf.Lineup, 0, lineup, 0, leftHalf.Lineup.Length);
                    // Copy the right half lineup to the right half of the resulting lineup
                    Array.Copy(rightHalf.Lineup, 0, lineup, half, rightHalf.Lineup.Length);

                    //Console.WriteLine("Yielding {0}", lineup.ToTournament());

                    yield return new SubResult(leftHalf.RemainingFighters, lineup);
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