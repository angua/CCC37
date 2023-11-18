using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using CommonWPF;
using RockPaperScissors;

namespace RockPaperScissorsUI
{
    class MainViewModel : ViewModelBase
    {
        private List<Tournament> _tournaments = new();
        private int _maxTournamentNumber;

        private List<string> _tournamentRounds = new();

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


        public int FileNumber
        {
            get => GetValue<int>();
            set
            {
                if (value < 1)
                {
                    value = 1;
                }
                if (value > 5)
                {
                    value = 5;
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

        public VisualTournament? CurrentVisualTournament
        {
            get => GetValue<VisualTournament>();
            set => SetValue(value);
        }

        public ObservableCollection<object> VisualTournament { get; set; } = new();

        public string Winner
        {
            get => GetValue<string>();
            set => SetValue(value);
        }



        public MainViewModel()
        {
            Winner = string.Empty;
            Level = 6;

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
            return true;
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

            if (Level == 5)
            {

                var result = TournamentHandler.GuessSolution(CurrentVisualTournament.CurrentTournament);
                _tournamentRounds = TournamentHandler.CreateRounds(result);
                Winner = _tournamentRounds.Last().First().ToString();
            }
            else
            {
                _tournamentRounds.Add(CurrentVisualTournament.CurrentTournament.Lineup);
            }

            CreateVisuals(_tournamentRounds);
        }


        private void CreateVisuals(List<string> tournamentRounds)
        {
            VisualTournament.Clear();
            var maxLength = tournamentRounds.Max(t => t.Length);

            for (int round = 0; round < tournamentRounds.Count; round++)
            {
                var currentRound = tournamentRounds[round];

                var fraction = maxLength / currentRound.Length;

                for (int i = 0; i < currentRound.Length; i++)
                {
                    var fighter = currentRound[i];
                    var x = (0.5 + i) * fraction;

                    var unknown = false;
                    if (currentRound.Length == maxLength && 
                        CurrentVisualTournament.CurrentTournament.Lineup.Length == maxLength &&
                        CurrentVisualTournament.CurrentTournament.Lineup[i] == 'X')
                    {
                        unknown = true;
                    }


                    // fighter box
                    VisualTournament.Add(new VisualFighter()
                    {
                        FighterType = fighter,
                        PositionX = x,
                        PositionY = round,
                        IsUnknown = unknown
                    });

                    // connection line to box below
                    if (currentRound.Length > 1)
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
}
