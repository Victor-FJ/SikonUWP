using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ModelLibrary.Model;
using SikonUWP.Common;
using SikonUWP.Handlers;

namespace SikonUWP.ViewModel
{
    class ParticipantCreatorViewModel
    {
        public ICommand CreateParticipantCommand { get; set; }

        public ParticipantHandler ParticipantHandler { get; set; }

        private Participant _newParticipant;

        public Participant NewParticipant
        {
            get { return _newParticipant; }
            set { _newParticipant = value; }
        }

        public ParticipantCreatorViewModel()
        {
            NewParticipant = new Participant();
            CreateParticipantCommand = new RelayCommand(()=> ParticipantHandler.CreateParticipant(NewParticipant), () => NewParticipant != null);
        }

    }
}
