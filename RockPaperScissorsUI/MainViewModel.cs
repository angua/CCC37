using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonWPF;
using RockPaperScissors;

namespace RockPaperScissorsUI
{
    class MainViewModel : ViewModelBase
    {
        private List<Tournament> _tournaments = new();

        public int FileNumber
        {
            get => GetValue<int>();
            set
            {
                SetValue(value);
                TournamentNumber = 1;
            }
        }

        public int TournamentNumber
        {
            get => GetValue<int>();
            set
            {
                SetValue(value);
                CurrentTournament = _tournaments.FirstOrDefault(t => t.FileNumber == FileNumber && t.TournamentNumber == value);
            }
        }

        public Tournament? CurrentTournament
        {
            get => GetValue<Tournament>();
            set => SetValue(value);
        }



        public MainViewModel()
        {
            _tournaments = TournamentHandler.ParseTournaments(5);

            FileNumber = 1;
            TournamentNumber = 1;

        }

    }
}
