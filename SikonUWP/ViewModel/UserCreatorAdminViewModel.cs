using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;
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
    class UserCreatorAdminViewModel: INotifyPropertyChanged
    {
        public ObservableCollection<string> UserTypeList { get; set; }
        public List<Participant.PersonType> PersonTypeList { get; set; }

        private string _userType;

        public string UserType
        {
            get { return _userType; }
            set
            {
                _userType = value;
                IsAdminSelected = value == "Admin" ? "Visible" : "Collapsed";
                IsSpeakerSelected = value == "Speaker" ? "Visible" : "Collapsed";
                IsParticipantSelected = value == "Participant" ? "Visible" : "Collapsed";
                OnPropertyChanged();
            }
        }


        private ParticipantHandler participantHandler = new ParticipantHandler();

        private SpeakerHandler speakerHandler = new SpeakerHandler();

        private AdminHandler adminHandler = new AdminHandler();

        private ICommand _createParticipantCommand;
        public ICommand CreateParticipantCommand
        {
            get { return _createParticipantCommand; }
            set
            {
                _createParticipantCommand = value;

                //skal flyttes til metoden UserTypeButtonPress
                //if (UserType == "participant")
                //{
                //    _createUserCommand = new RelayCommand(()=> participantHandler.CreateParticipant(NewParticipant), ()=> NewUser != null);
                //}else if (UserType == "speaker")
                //{
                //    _createUserCommand = new RelayCommand(() => speakerHandler.CreateSpeaker(NewSpeaker), () => NewUser != null);
                //}else if (UserType == "Admin")
                //{
                //    _createUserCommand = new RelayCommand(()=> adminHandler.CreateAdmin(NewAdmin),()=> NewUser != null);
                //}
                OnPropertyChanged();
            }
        }

        private ICommand _createAdminCommand;

        public ICommand CreateAdminCommand
        {
            get { return _createAdminCommand; }
            set { _createAdminCommand = value; }
        }


        private ICommand _createSpeakerCommand;

        public ICommand CreateSpeakerCommand
        {
            get { return _createSpeakerCommand; }
            set { _createSpeakerCommand = value; }
        }



        //private ICommand _updateUserCommand;

        //public ICommand UpdateUserCommand
        //{
        //    get { return _updateUserCommand; }
        //    set
        //    {
        //        _updateUserCommand = value;
        //        OnPropertyChanged();
        //    }
        //}

        //private ICommand _deleteUserCommand;

        //public ICommand DeleteUserCommand
        //{
        //    get { return _deleteUserCommand; }
        //    set { _deleteUserCommand = value; }
        //}


        private User _newUser;

        public User NewUser
        {
            get { return _newUser; }
            set { _newUser = value;  OnPropertyChanged(); }
        }

        private Participant _newParticipant;

        public Participant NewParticipant
        {
            get { return _newParticipant; }
            set
            {
                _newParticipant = value;
                if (NewUser != null)
                {
                    _newParticipant.UserName = NewUser.UserName ;
                    _newParticipant.Password = NewUser.Password;
                }
                OnPropertyChanged();
            }
        }

        private Speaker _newSpeaker;

        public Speaker NewSpeaker
        {
            get { return _newSpeaker; }
            set
            {
                _newSpeaker = value;
                if (NewUser != null)
                {
                    _newSpeaker.UserName = NewUser.UserName;
                    _newSpeaker.Password = NewUser.Password;
                }
                OnPropertyChanged();
            }
        }

        private Admin _newAdmin;

        public Admin NewAdmin
        {
            get { return _newAdmin; }
            set
            {
                _newAdmin = value;
                if (NewUser != null)
                {
                    _newAdmin.UserName = NewUser.UserName;
                    _newAdmin.Password = NewUser.Password;
                }
                OnPropertyChanged();
            }
        }

        private string _isAdminSelected = "Collapsed";

        public string IsAdminSelected
        {
            get { return _isAdminSelected; }
            set
            {
                _isAdminSelected = value;
                OnPropertyChanged();
                ((RelayCommand)_createAdminCommand).RaiseCanExecuteChanged();
            }
        }

        private string _isSpeakerSelected = "Collapsed";

        public string IsSpeakerSelected
        {
            get { return _isSpeakerSelected; }
            set
            {
                _isSpeakerSelected = value; 
                OnPropertyChanged();
                ((RelayCommand)_createSpeakerCommand).RaiseCanExecuteChanged();
            }
        }

        private string _isParticipantSelected = "Collapsed";

        public string IsParticipantSelected
        {
            get { return _isParticipantSelected; }
            set
            {
                _isParticipantSelected = value;
                OnPropertyChanged();
                ((RelayCommand)_createParticipantCommand).RaiseCanExecuteChanged();
            }
        }






        public UserCreatorAdminViewModel()
        {
            
            PersonTypeList = Enum.GetValues(typeof(Participant.PersonType)).OfType<Participant.PersonType>().ToList();

            NewUser = new User();
            NewParticipant = new Participant();
            NewAdmin = new Admin();
            NewSpeaker = new Speaker();

            UserTypeList = new ObservableCollection<string>();
            UserTypeList.Add("Admin");
            UserTypeList.Add("Speaker");
            UserTypeList.Add("Participant");

            CreateParticipantCommand = new RelayCommand(() => participantHandler.CreateParticipant(NewParticipant), ()=> NewParticipant != null);
            CreateSpeakerCommand = new RelayCommand(()=> speakerHandler.CreateSpeaker(NewSpeaker), ()=> NewSpeaker != null);
            CreateAdminCommand = new RelayCommand(()=> adminHandler.CreateAdmin(NewAdmin), ()=> NewAdmin != null);
        }





        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
