using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using ModelLibrary.Model;
using SikonUWP.Annotations;
using SikonUWP.Common;
using SikonUWP.Handlers;
using SikonUWP.Model;

namespace SikonUWP.ViewModel
{
    class UserCreatorAdminViewModel: INotifyPropertyChanged
    {
        public ObservableCollection<string> UserTypeList { get; set; }
        public List<Participant.PersonType> PersonTypeList { get; set; }
        public List<User> UserList { get; set; }
        public List<string> UserNameList { get; set; }

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



        private ICommand _getImageCommand;

        public ICommand GetImageCommand
        {
            get { return _getImageCommand; }
            set { _getImageCommand = value; }
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

        public string Username { get; set; }
        public string Password { get; set; }

        private Participant.PersonType _personType;

        public Participant.PersonType PersonType
        {
            get { return _personType; }
            set { _personType = value; ((RelayCommand)CreateParticipantCommand).RaiseCanExecuteChanged(); }
        }

        private string _phoneNumber;

        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set { _phoneNumber = value; ((RelayCommand)CreateAdminCommand).RaiseCanExecuteChanged(); OnPropertyChanged(); }
        }

        private string _fullName;

        public string FullName
        {
            get { return _fullName; }
            set { _fullName = value; ((RelayCommand)CreateSpeakerCommand).RaiseCanExecuteChanged(); }
        }

        private string _description;

        public string Description
        {
            get { return _description; }
            set { _description = value; ((RelayCommand)CreateSpeakerCommand).RaiseCanExecuteChanged(); }
        }

        private BitmapImage _imageView;

        public BitmapImage ImageView
        {
            get { return _imageView; }
            set { _imageView = value; OnPropertyChanged();}
        }

        private StorageFile _image;

        public StorageFile Image
        {
            get { return _image; }
            set { _image = value; ((RelayCommand)CreateSpeakerCommand).RaiseCanExecuteChanged(); }
        }

       
        public ImageSingleton ImageCatalog { get; set; }
        





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

        private Speaker _newSpeaker;

        public Speaker NewSpeaker
        {
            get { return _newSpeaker; }
            set
            {
                _newSpeaker = value;
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
        private bool IsBasicUserNull()
        {
            return Username != null && Password != null;
        }
        private bool IsParticipantNull()
        {
            return IsBasicUserNull() && PersonType != Participant.PersonType.Vælg_type;
        }
        private bool IsSpeakerNull()
        {
            return IsBasicUserNull() && FullName != null && Description != null && Image != null;
        }
        private bool IsAdminNull()
        {
            return IsBasicUserNull() && PhoneNumber != null;
        }





        public UserCreatorAdminViewModel()
        {
            
            
            PersonTypeList = Enum.GetValues(typeof(Participant.PersonType)).OfType<Participant.PersonType>().ToList();

            NewParticipant = new Participant();
            NewAdmin = new Admin();
            NewSpeaker = new Speaker();
            ImageCatalog = ImageSingleton.Instance;


            UserTypeList = new ObservableCollection<string>();
            UserTypeList.Add("Admin");
            UserTypeList.Add("Speaker");
            UserTypeList.Add("Participant");

            CreateParticipantCommand = new RelayCommand(CreateParticipant, IsParticipantNull);
            CreateSpeakerCommand = new RelayCommand(CreateSpeaker, IsSpeakerNull);
            CreateAdminCommand = new RelayCommand(CreateAdmin, IsAdminNull);
            GetImageCommand = new RelayCommand(GetImage);
        }


        private async void CreateParticipant()
        {
            
            if (PersonType == Participant.PersonType.Vælg_type)
            {
                await MessageDialogUtil.MessageDialogAsync("Ingen PersonType",
                    "Der er ikke valgt en PersonType\nVælg venligst persontype");
            }else
            {
                FillUserList();
                NewParticipant = new Participant(Username, Password, PersonType);
                if (!UserNameList.Contains(NewParticipant.UserName)) 
                {
                    participantHandler.CreateParticipant(NewParticipant);
                }
                else
                 await MessageDialogUtil.MessageDialogAsync("Username Already taken",
                    "brugernavnet du har angivet er allerede i brug \nbenyt venligst et andet brugernavn");
            }
        }

        private async void CreateSpeaker()
        {
            FillUserList();
            NewSpeaker = new Speaker(Username, Password, FullName, Description, Image.Name);
            if (!ImageCatalog.ImageCatalog.Dictionary.Keys.Contains(Image.Name))
            {
                await ImageCatalog.ImageCatalog.AddImage(Image, Image.Name);
                if (!UserNameList.Contains(NewSpeaker.UserName))
                {
                   speakerHandler.CreateSpeaker(NewSpeaker);
                }
                else
                    await MessageDialogUtil.MessageDialogAsync("Username Already taken",
                        "brugernavnet du har angivet er allerede i brug \nbenyt venligst et andet brugernavn");

            }
            else
            {
                await MessageDialogUtil.MessageDialogAsync("Billede eksisterer allerede",
                    "navnet af det valgte billede eksistere allerede i systemet vælg venligst et andet billede eller ændre navnet på din billedfil");
            }
            
            
        }

        private async void CreateAdmin()
        {
            
            if (Regex.Matches(_phoneNumber, @"[a-zA-Z]").Count > 0)
            {
                await MessageDialogUtil.MessageDialogAsync("Bogstav i dit telefon nummer",
                    "Der er et eller flere bogstaver i dit telefon nummer\nprøv venligst igen kun med tal");
            }else 
            {
                FillUserList();
                NewAdmin = new Admin(Username, Password, PhoneNumber);
                if (!UserNameList.Contains(NewAdmin.UserName))
                {
                    adminHandler.CreateAdmin(NewAdmin);
                }
                else
                    await MessageDialogUtil.MessageDialogAsync("Username Already taken",
                        "brugernavnet du har angivet er allerede i brug \nbenyt venligst et andet brugernavn");

            }

        }

        public async void GetImage()
        {
            Image = await ImageCatalog.ImageCatalog.PickSingleImage();
            if (Image != null)
            {
                ImageView = await ImageCatalog.ImageCatalog.AsBitmapImage(Image);
            }
            else
                await MessageDialogUtil.MessageDialogAsync("Forkert filtype", "Kunne ikke hente billedet");
        }


        private async void FillUserList()
        {
            await UserCatalogSingleton.Instance.LoadUsers();
            UserList = UserCatalogSingleton.Instance.Users.ToList();
                
            UserNameList = new List<string>();
            foreach (User user in UserList)
            {
                UserNameList.Add(user.UserName);
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
