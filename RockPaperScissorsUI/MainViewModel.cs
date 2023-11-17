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
            }
        }

        public VisualTournament? CurrentVisualTournament
        {
            get => GetValue<VisualTournament>();
            set => SetValue(value);
        }

        public ObservableCollection<object> VisualTournament { get; set; } = new();


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

            CreateVisuals(_tournamentRounds);

        }


        private void CreateVisuals(List<string> tournamentRounds)
        {
            VisualTournament.Clear();
            var maxLength = tournamentRounds.Max(t => t.Length);

            for (int round = 0; round < tournamentRounds.Count; round++)
            {
                var currentRound = tournamentRounds[round];

                var half = currentRound.Length / 2;

                var fraction =  maxLength / currentRound.Length;

                for (int i = 0; i < currentRound.Length; i++)
                {
                    var fighter = currentRound[i];

                    var x = fraction * i + 0.5 * fraction;

                    VisualTournament.Add(new VisualFighter()
                    {
                        FighterType = fighter,
                        PositionX = x,
                        PositionY = round
                    });

                }

            }
        }

    }
}
