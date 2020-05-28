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
using SikonUWP.Model;
using SikonUWP.View;

namespace SikonUWP.ViewModel
{
    class UserLogin_CreateViewModel: INotifyPropertyChanged
    {

        private ParticipantHandler participantHandler = new ParticipantHandler();
        public List<Participant.PersonType> PersonTypeList { get; set; }
        private List<User> UserList = new List<User>();
        private Dictionary<string, string> userDictionary = new Dictionary<string, string>();
        private List<string> UserNameList = new List<string>();


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

        private ICommand _logInCommand;

        public ICommand LogInCommand
        {
            get { return _logInCommand; }
            set { _logInCommand = value; }
        }





        private string _username;

        public string Username
        {
            get { return _username; }
            set { _username = value; OnPropertyChanged(); ((RelayCommand)LogInCommand).RaiseCanExecuteChanged(); ((RelayCommand)CreateParticipantCommand).RaiseCanExecuteChanged(); }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged(); ((RelayCommand)LogInCommand).RaiseCanExecuteChanged(); ((RelayCommand)CreateParticipantCommand).RaiseCanExecuteChanged(); }
        }

        private Participant.PersonType _personType;

        public Participant.PersonType PersonType
        {
            get { return _personType; }
            set { _personType = value; OnPropertyChanged(); ((RelayCommand)CreateParticipantCommand).RaiseCanExecuteChanged(); }
        }
        private string _mode1 = "Visible";

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

        private bool IsNewParticipantNull()
        {
            return Username != null && Password != null && PersonType != Participant.PersonType.Vælg_type;
        }

        private bool IsLoginEmpty()
        {
            return Username != null && Password != null;
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

        public static UserLogin_CreateViewModel vmInstance { get; set; }
        public UserLogin_CreateViewModel()
        {
            PersonTypeList = Enum.GetValues(typeof(Participant.PersonType)).OfType<Participant.PersonType>().ToList();
            vmInstance = this;
            NewParticipant = new Participant();
            CreateParticipantCommand = new RelayCommand(CreateParticipant, IsNewParticipantNull);
            ChangeModeCommand = new RelayCommand(ChangeMode);
            LogInCommand = new RelayCommand(LogIn, IsLoginEmpty);
        }


        private async void CreateParticipant()
        {
            if (PersonType == Participant.PersonType.Vælg_type)
            {
                await MessageDialogUtil.MessageDialogAsync("Ingen PersonType",
                    "Der er ikke valgt en PersonType\nVælg venligst persontype");
            }
            else
            {
                await FillUserList();
                NewParticipant = new Participant(Username, Password, PersonType);
                if (!userDictionary.Keys.Contains(NewParticipant.UserName))
                {
                    participantHandler.CreateParticipant(NewParticipant);
                }
                else
                    await MessageDialogUtil.MessageDialogAsync("Username Already taken",
                        "brugernavnet du har angivet er allerede i brug \nbenyt venligst et andet brugernavn");
            }


        }


        public async void LogIn()
        { 
            await FillUserList();
            if (userDictionary.ContainsKey(Username) && userDictionary[Username] == Password)
            {
                MainViewModel.Instance.LoggedUser = new User(Username, Password);
                //return true;
            }else if (!UserNameList.Contains(Username))
            {
                await MessageDialogUtil.MessageDialogAsync("Brugernavnet er forkert",
                    "brugernavnet du har angivet eksisterer ikke \nprøv venligst et andet brugernavn");
                //return false;
            }
            else
            {
                await MessageDialogUtil.MessageDialogAsync("Password forkert",
                    "det angivede password er forkert \nprøv venligst et andet password");
                //return false;
            }
            
        }

        private void ChangeMode()
        {
            if (Mode1 == "Collapsed")
            {
                Mode1 = "Visible";
                Mode2 = "Collapsed";
            }else if (Mode1 == "Visible")
            {
                Mode1 = "Collapsed";
                Mode2 = "Visible";
            }
        }


        private async Task FillUserList()
        {
            userDictionary.Clear();
            await UserCatalogSingleton.Instance.LoadUsers();
            UserList = UserCatalogSingleton.Instance.Users.ToList();

            UserNameList = new List<string>();
            foreach (User user in UserList)
            {
                UserNameList.Add(user.UserName);
                userDictionary.Add(user.UserName, user.Password);
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
