using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockPaperScissors;

public class Tournament
{
    public int FileNumber { get; set; }

    public int TournamentNumber { get; set; }


    public FighterSet Set { get; set; }

    public int FigherCount { get; set; }
}
