using System.Collections.ObjectModel;
using CommonWPF;
using RockPaperScissors;

namespace RockPaperScissorsUI;

internal class VisualTournament : ViewModelBase
{
    public Tournament CurrentTournament { get; private set; }
    public VisualTournament(Tournament tournament)
    {
       CurrentTournament = tournament;

        for (int i = 0; i < 5; i++)
        {
            VisualSet.Add(CurrentTournament.Set[(Fighter)i]);
        }
    }

    public ObservableCollection<int> VisualSet { get; set; } = new();

    public FighterSet Set => CurrentTournament.Set;

    public int FighterCount => CurrentTournament.FigherCount;

}
