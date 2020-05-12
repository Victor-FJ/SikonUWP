using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ModelLibrary.Model;
using SikonUWP.Annotations;
using SikonUWP.Common;
using SikonUWP.Handlers;

namespace SikonUWP.ViewModel
{
    class UserLogin_CreateViewModel: INotifyPropertyChanged
    {

        private ParticipantHandler participantHandler = new ParticipantHandler();
        public List<Participant.PersonType> PersonTypeList { get; set; }


        private ICommand _createParticipantCommand;
        public ICommand CreateParticipantCommand
        {
            get { return _createParticipantCommand; }
            set
            {
                _createParticipantCommand = value;


                OnPropertyChanged();
            }
        }
        private ICommand _changeModeCommand;

        public ICommand ChangeModeCommand
        {
            get { return _changeModeCommand; }
            set { _changeModeCommand = value; }
        }





        public string Username { get; set; }
        public string Password { get; set; }
        public Participant.PersonType PersonType { get; set; }
        private string _mode1;

        public string Mode1
        {
            get { return _mode1; }
            set { _mode1 = value;
                OnPropertyChanged();
            }
        }

        private string _mode2 = "Collapsed";

        public string Mode2
        {
            get { return _mode2; }
            set
            {
                _mode2 = value;
                OnPropertyChanged();
            }

        }





        private Participant _newParticipant;

        public Participant NewParticipant
        {
            get { return _newParticipant; }
            set
            {
                _newParticipant = value;
                OnPropertyChanged();
            }
        }


        public UserLogin_CreateViewModel()
        {
            PersonTypeList = Enum.GetValues(typeof(Participant.PersonType)).OfType<Participant.PersonType>().ToList();

            NewParticipant = new Participant();
            CreateParticipantCommand = new RelayCommand(CreateParticipant, () => NewParticipant != null);
            ChangeModeCommand = new RelayCommand(ChangeMode);
        }


        private void CreateParticipant()
        {
            NewParticipant = new Participant(Username, Password, PersonType);
            participantHandler.CreateParticipant(NewParticipant);
        }

        private void ChangeMode()
        {
           Mode1 = Mode1 == "Collapsed" ? "Visible" : "Collapsed";
           Mode2 = Mode1 != "Collapsed" ? "Visible" : "Collapsed";
        }




        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
