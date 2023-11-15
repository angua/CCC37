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

}
