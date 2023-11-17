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
                DoShowSolution();
            }
        }

        public VisualTournament? CurrentVisualTournament
        {
            get => GetValue<VisualTournament>();
            set => SetValue(value);
        }

        public ObservableCollection<object> VisualTournament { get; set; } = new();

        public char Winner
        {
            get => GetValue<char>();
            set => SetValue(value);
        }



        public MainViewModel()
        {
            _tournaments = TournamentHandler.ParseTournaments(5);

            FileNumber = 1;
            TournamentNumber = 1;



            PreviousInput = new RelayCommand(CanPreviousInput, DoPreviousInput);
            NextInput = new RelayCommand(CanNextInput, DoNextInput);
            ShowSolution = new RelayCommand(CanShowSolution, DoShowSolution);

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

        public RelayCommand ShowSolution { get; }
        public bool CanShowSolution()
        {
            return true;
        }
        public void DoShowSolution()
        {
            _tournamentRounds.Clear();

            var result = TournamentHandler.GuessSolution(CurrentVisualTournament.CurrentTournament);
            _tournamentRounds = TournamentHandler.CreateRounds(result);
            Winner = _tournamentRounds.Last().First();

            CreateVisuals(_tournamentRounds);

        }


        private void CreateVisuals(List<string> tournamentRounds)
        {
            VisualTournament.Clear();
            var maxLength = tournamentRounds.Max(t => t.Length);

            for (int round = 0; round < tournamentRounds.Count; round++)
            {
                var currentRound = tournamentRounds[round];

                var fraction =  maxLength / currentRound.Length;

                for (int i = 0; i < currentRound.Length; i++)
                {
                    var fighter = currentRound[i];

                    var x = (0.5 + i) * fraction;

                    // fighter box
                    VisualTournament.Add(new VisualFighter()
                    {
                        FighterType = fighter,
                        PositionX = x,
                        PositionY = round
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
