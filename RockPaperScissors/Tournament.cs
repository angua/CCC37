﻿namespace RockPaperScissors;

public class Tournament
{
    public int Level { get; set; }
    public int FileNumber { get; set; }
    public int TournamentNumber { get; set; }


    public FighterSet Set { get; set; }

    public string Lineup { get; set; }

    public List<List<string>> Rounds { get; set; } = new();

    public List<List<HashSet<char>>> PossibleRounds { get; set; } = new();


    public int FigherCount { get; set; }
}
