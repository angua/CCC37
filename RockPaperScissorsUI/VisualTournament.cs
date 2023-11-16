using System.Collections.ObjectModel;
using CommonWPF;
using RockPaperScissors;

namespace RockPaperScissorsUI;

internal class VisualTournament : ViewModelBase
{
    private Tournament _tournament;
    public VisualTournament(Tournament tournament)
    {
       _tournament = tournament;

        for (int i = 0; i < 5; i++)
        {
            VisualSet.Add(_tournament.Set[(Fighter)i]);
        }
    }

    public ObservableCollection<int> VisualSet { get; set; } = new();

    public FighterSet Set => _tournament.Set;

    public int FighterCount => _tournament.FigherCount;

}
