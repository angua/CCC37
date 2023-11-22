using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommonWPF;
using RockPaperScissors;

namespace RockPaperScissorsUI;

class MainViewModel : ViewModelBase
{
    private List<Tournament> _tournaments = new();
    private int _maxTournamentNumber;

    private List<List<string>> _tournamentRounds = new();


    public List<int> PossibleLevels { get; private set; } = Enumerable.Range(1, 7).ToList();
    public int Level
    {
        get => GetValue<int>();
        set
        {
            SetValue(value);
            _tournaments.Clear();
            _tournaments = TournamentHandler.ParseTournaments(value);

            FileNumber = 1;
            TournamentNumber = 1;
        }
    }


    public List<int> PossibleFileNumbers { get; private set; } = Enumerable.Range(1, 5).ToList();
    public int FileNumber
    {
        get => GetValue<int>();
        set
        {
            if (!PossibleFileNumbers.Contains(value))
            {
                value = 1;
            }
            
            SetValue(value);
            TournamentNumber = 1;
            _maxTournamentNumber = _tournaments.Where(t => t.FileNumber == value).Count();
        }
    }

    public int TournamentNumber
    {
        get => GetValue<int>();
        set
        {
            SetValue(value);
            CurrentVisualTournament = new VisualTournament(_tournaments.FirstOrDefault(t => t.FileNumber == FileNumber && t.TournamentNumber == value));
            ShowTournament();
        }
    }

    public ObservableCollection<object> VisualTournament { get; set; } = new();
    public VisualTournament? CurrentVisualTournament
    {
        get => GetValue<VisualTournament>();
        set => SetValue(value);
    }

    public string Winner
    {
        get => GetValue<string>();
        set => SetValue(value);
    }


    public MainViewModel()
    {
        Winner = string.Empty;
        Level = 7;

        PreviousInput = new RelayCommand(CanPreviousInput, DoPreviousInput);
        NextInput = new RelayCommand(CanNextInput, DoNextInput);
        Solve = new RelayCommand(CanSolve, DoSolve);
    }

    public RelayCommand PreviousInput { get; }
    public bool CanPreviousInput()
    {
        return TournamentNumber > 1;
    }
    public void DoPreviousInput()
    {
        TournamentNumber--;
    }

    public RelayCommand NextInput { get; }
    public bool CanNextInput()
    {
        return TournamentNumber < _maxTournamentNumber;
    }
    public void DoNextInput()
    {
        TournamentNumber++;
    }

    public RelayCommand Solve { get; }
    public bool CanSolve()
    {
        return Level != 1 &&
               Level != 2 &&
               Level != 3 &&
               Level != 4 &&
               Level != 5;
    }
    public void DoSolve()
    {
        _tournamentRounds.Clear();
        var solution = TournamentHandler.Solve(CurrentVisualTournament.CurrentTournament);
        _tournamentRounds = TournamentHandler.CreateRounds(solution);
        Winner = _tournamentRounds.Last().First().ToString();

        CreateVisuals(_tournamentRounds);
    }


    public void ShowTournament()
    {
        _tournamentRounds.Clear();
        Winner = string.Empty;

        if (Level == 3 || Level == 4 || Level == 5)
        {
            var lineup = TournamentHandler.Solve(CurrentVisualTournament.CurrentTournament);
            _tournamentRounds = TournamentHandler.CreateRounds(lineup);
            Winner = _tournamentRounds.Last().First().ToString();
        }
        else if (Level == 7)
        {
            foreach (var round in CurrentVisualTournament.CurrentTournament.PossibleRounds)
            {
                _tournamentRounds.Add(round.Select(f => string.Join("", f)).ToList());
            }
        }
        else
        {
            foreach (var round in CurrentVisualTournament.CurrentTournament.Rounds)
            {
                _tournamentRounds.Add(round);
            }
        }

        CreateVisuals(_tournamentRounds);
    }


    private void CreateVisuals(List<List<string>> tournamentRounds)
    {
        VisualTournament.Clear();
        var maxLength = tournamentRounds.Max(t => t.Count);

        for (int round = 0; round < tournamentRounds.Count; round++)
        {
            var currentRound = tournamentRounds[round];

            var fraction = maxLength / currentRound.Count;

            for (int i = 0; i < currentRound.Count; i++)
            {
                var fighter = currentRound[i];
                var x = (0.5 + i) * fraction;

                var unknownOrAvailable = UnknownOrAvailable.None;

                var originalRound = CurrentVisualTournament.CurrentTournament.Rounds.FirstOrDefault(r => r.Count == currentRound.Count);

                if (originalRound != null)
                {
                    if (originalRound[i] == "X")
                    {
                        unknownOrAvailable = UnknownOrAvailable.Available;
                    }
                    if (originalRound[i] == "Z")
                    {
                        unknownOrAvailable = UnknownOrAvailable.Unknown;
                    }
                }

                // fighter box
                VisualTournament.Add(new VisualFighter()
                {
                    FighterType = fighter,
                    PositionX = x,
                    PositionY = round,
                    IsUnknownOrAvailable = unknownOrAvailable
                });

                // connection line to box below
                if (currentRound.Count > 1)
                {
                    // will be rounded down to int
                    var elementBelow = i / 2;
                    var fractionbelow = fraction * 2;
                    var xElementBelow = (0.5 + elementBelow) * fractionbelow;

                    VisualTournament.Add(new ConnectionLine()
                    {
                        FighterType = fighter,
                        StartPositionX = x,
                        StartPositionY = round,
                        EndPositionX = xElementBelow,
                        EndPositionY = round + 1
                    });
                }

            }

        }
    }

}
