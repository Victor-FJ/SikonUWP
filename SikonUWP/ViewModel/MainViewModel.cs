using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using ModelLibrary.Annotations;
using ModelLibrary.Exceptions;
using ModelLibrary.Model;
using SikonUWP.Common;
using SikonUWP.Model;
using SikonUWP.Persistency;
using SikonUWP.View;

namespace SikonUWP.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly Frame _frame;
        private readonly NavigationView _navigationView;

        public static MainViewModel Instance { get; private set; }

        #region ControlProperties

        private readonly Dictionary<Participant.PersonType, string> _participantProfiles = MakePartProf();

        public bool LoadRing { get; set; }
        private string _loadText;

        public string LoadText
        {
            get => _loadText;
            set
            {
                if (value == null)
                {
                    _loadText = "Færdig";
                    LoadRing = false;
                }
                else if (value.Contains("Fejl"))
                {
                    _loadText = value;
                    LoadRing = false;
                }
                else
                {
                    _loadText = value;
                    LoadRing = true;
                }
                OnPropertyChanged(nameof(LoadRing));
                OnPropertyChanged();
            }
        }


        private User _loggedUser;
        public User LoggedUser
        {
            get { return _loggedUser; }
            set
            {
                _loggedUser = value;
                SetProfile();
                CheckAccess();
            }
        }

        public Admin IsAdmin { get; set; }
        public Speaker IsSpeaker { get; set; }
        public Participant IsParticipant { get; set; }

        public object Profil { get; set; }
        public object Badge { get; set; }
        public string UserName { get; set; }


        public string SearchString { get; set; }
        private bool _isChosingSuggestion;

        #endregion

        public List<Event> SuggestedEvents { get; set; }

        public ICommand SearchCommand { get; set; }
        public ICommand SuggestionChosenCommand { get; set; }
        public ICommand QuerySubmittedCommand { get; set; }

        public ICommand ReloadCommand { get; set; }

        public MainViewModel(Frame mainPageFrame, NavigationView navigationView)
        {
            _frame = mainPageFrame;
            _navigationView = navigationView;
            Instance = this;


            Load();

            SearchCommand = new RelayCommand(Search);
            SuggestionChosenCommand = new RelayCommand(SuggestionChosen);
            QuerySubmittedCommand = new RelayCommand(QuerySubmitted);
            ReloadCommand = new RelayCommand(async () =>
            {
                bool ok = await Reload();
                if (ok) NavigateToPage(mainPageFrame.CurrentSourcePageType);
            });
        }

        #region Navigation

        /// <summary>
        /// Navigates to a page
        /// </summary>
        /// <param name="pageType">The type of the page</param>
        public void NavigateToPage(Type pageType)
        {
            _frame.Navigate(pageType);
            UptNaviCursor(pageType);
            if (_frame.CanGoBack)
                _navigationView.IsBackEnabled = true;
        }

        public void UptNaviCursor(Type pageType)
        {
            foreach (object menuItem in _navigationView.MenuItems)
                if (menuItem is NavigationViewItem navigationViewItem
                    && navigationViewItem.Tag is Type naviPageType
                    && naviPageType == pageType)
                    _navigationView.SelectedItem = navigationViewItem;
        }

        #endregion

        #region LoadMethods

        private async void Load()
        {
            await Reload();
            if (_frame.CurrentSourcePageType == null)
                NavigateToPage(typeof(EventHomePage));
        }

        public async Task<bool> Reload()
        {
            try
            {
                LoadText = "1/8 - Opretter forbindelse";
                bool ok = await PersistencyManager.TryOpenConn();
                if (ok)
                {
                    LoadText = "2/8 - Henter billeder";
                    await ImageSingleton.Instance.ImageCatalog.SyncImages();
                    LoadText = "3/8 - Henter lokaler";
                    await RoomCatalogSingleton.Instance.LoadRooms();
                    LoadText = "4/8 - Henter deltagere";
                    await ParticipantCatalogSingleton.Instance.LoadParticipants();
                    LoadText = "5/8 - Henter oplægsholdere";
                    await SpeakerCatalogSingleton.Instance.LoadSpeakers();
                    LoadText = "6/8 - Henter administratore";
                    await AdminCatalogSingleton.Instance.LoadAdmins();
                    LoadText = "7/8 - Henter begivenheder";
                    await EventSingleton.Instance.EventCatalog.Load();
                    LoadText = "8/8 - Henter registration";
                    await RegistrationSingleton.Instance.Load();
                    LoadText = null;
                }
                else
                {
                    LoadText = "Fejl";
                    await MessageDialogUtil.MessageDialogAsync(PersistencyManager.FileName, "ConnectionString er forkert");
                }
                return ok;
            }
            catch (HttpRequestException)
            {
                LoadText = "Fejl";
                await MessageDialogUtil.MessageDialogAsync(PersistencyManager.FileName, PersistencyManager.Message);
                return false;
            }
        }

        #endregion

        #region LoginUser

        private void CheckAccess()
        {
            if (IsAdmin != null && _navigationView.MenuItems.Count == 2)
            {
                _navigationView.MenuItems.Insert(0, new NavigationViewItemHeader() { Content = "Begivenheder" });
                _navigationView.MenuItems.Insert(2, new NavigationViewItem() { Icon = new SymbolIcon() {Symbol = Symbol.Edit}, Content = "Lav ny begivenhed", Tag = typeof(EventEditorPage)});
                _navigationView.MenuItems.Insert(3, new NavigationViewItemSeparator());
                _navigationView.MenuItems.Insert(4, new NavigationViewItemHeader() { Content = "Brugere" });
                _navigationView.MenuItems.Insert(6, new NavigationViewItem() { Icon = new SymbolIcon() { Symbol = Symbol.AddFriend }, Content = "Lav ny bruger", Tag = typeof(UserCreatorAdminPage)});
                _navigationView.MenuItems.Insert(7, new NavigationViewItemSeparator());
                _navigationView.MenuItems.Insert(8, new NavigationViewItemHeader() { Content = "Lokaler" });
                _navigationView.MenuItems.Insert(9, new NavigationViewItem() { Icon = new FontIcon() { Glyph = "\uE707" }, Content = "Lokale oversigt", Tag = typeof(RoomPage)});
            }
            else if (IsAdmin == null &&_navigationView.MenuItems.Count == 10)
            {
                _navigationView.MenuItems.RemoveAt(9);
                _navigationView.MenuItems.RemoveAt(8);
                _navigationView.MenuItems.RemoveAt(7);
                _navigationView.MenuItems.RemoveAt(6);
                _navigationView.MenuItems.RemoveAt(4);
                _navigationView.MenuItems.RemoveAt(3);
                _navigationView.MenuItems.RemoveAt(2);
                _navigationView.MenuItems.RemoveAt(0);
            }
            else if (_navigationView.MenuItems.Count != 2 && _navigationView.MenuItems.Count != 10)
                throw new BaseException("Adapting menu count is not correct");
        }

        private void SetProfile()
        {
            if (_loggedUser != null)
            {
                if (_loggedUser.UserName == null)
                    throw new BaseException("Of course this became a problem. If it can be a bug it will be");

                UserName = _loggedUser.UserName;

                List<object> possUserProfils = new List<object>();

                IsSpeaker =
                    SpeakerCatalogSingleton.Instance.Speakers.SingleOrDefault((x) =>
                        x.UserName == _loggedUser.UserName);
                if (IsSpeaker != null && ImageSingleton.Instance.ImageCatalog.Dictionary.ContainsKey(IsSpeaker.Image))
                    possUserProfils.Add(ImageSingleton.Instance.ImageCatalog.Dictionary[IsSpeaker.Image]);
                
                IsAdmin =
                    AdminCatalogSingleton.Instance.Admins.SingleOrDefault((x) =>
                        x.UserName == _loggedUser.UserName);
                if (IsAdmin != null)
                    possUserProfils.Add("../Assets/AdminProfil.png");
                
                IsParticipant =
                    ParticipantCatalogSingleton.Instance.Participants.SingleOrDefault((x) =>
                        x.UserName == _loggedUser.UserName);
                if (IsParticipant != null)
                    possUserProfils.Add(_participantProfiles[IsParticipant.Type]);

                if (possUserProfils.Count > 0)
                    Profil = possUserProfils[0];
                else
                    throw new BaseException("You have to be at least 1 type of user");
                Badge = null;
                if (possUserProfils.Count > 1)
                {
                    OnPropertyChanged(nameof(Badge));
                    Badge = possUserProfils[1];
                }
            }
            else
            {
                Profil = null;
                UserName = null;
            }
            OnPropertyChanged(nameof(Profil));
            OnPropertyChanged(nameof(Badge));
            OnPropertyChanged(nameof(UserName));
        }

        private static Dictionary<Participant.PersonType, string> MakePartProf()
        {
            Dictionary<Participant.PersonType, string> partProfs = Enum.GetValues(typeof(Participant.PersonType)).OfType<Participant.PersonType>().ToDictionary(x => x, x => (string)null);
            partProfs[Participant.PersonType.Autist] = "../Assets/AutistProfil.png";
            partProfs[Participant.PersonType.Fagperson] = "../Assets/ProfessionalProfil.png";
            partProfs[Participant.PersonType.ForældreAfAutist] = "../Assets/ParentProfil.png";
            partProfs[Participant.PersonType.Psykolog] = "../Assets/PsychologistProfil.png";
            partProfs[Participant.PersonType.Studerende] = "../Assets/StudentProfil.png";
            return partProfs;
        }

        #endregion

        #region AutoSugestBox

        public void Search(object parameter)
        {
            AutoSuggestBoxTextChangedEventArgs args = (AutoSuggestBoxTextChangedEventArgs) parameter;
            if (!_isChosingSuggestion)
            {
                SuggestedEvents = (from @event in EventSingleton.Instance.EventCatalog.Collection
                    where @event.Title.ToLower().Contains(SearchString.ToLower())
                    select @event).ToList();
                OnPropertyChanged(nameof(SuggestedEvents));
            }
            else
                _isChosingSuggestion = false;
        }

        public void SuggestionChosen(object parameter)
        {
            AutoSuggestBoxSuggestionChosenEventArgs args = (AutoSuggestBoxSuggestionChosenEventArgs) parameter;
            SearchString = ((Event) args.SelectedItem).Title;
            OnPropertyChanged(nameof(SearchString));
            _isChosingSuggestion = true;
        }

        public void QuerySubmitted(object parameter)
        {
            AutoSuggestBoxQuerySubmittedEventArgs args = (AutoSuggestBoxQuerySubmittedEventArgs) parameter;
            if (args.ChosenSuggestion != null)
            {
                EventSingleton.Instance.ViewedEvent = (Event) args.ChosenSuggestion;
                NavigateToPage(typeof(EventPage));
            }
            else
            {
                Event @event = EventSingleton.Instance.EventCatalog.Collection.SingleOrDefault(x => x.Title == SearchString);
                if (@event != null)
                {
                    EventSingleton.Instance.ViewedEvent = @event;
                    NavigateToPage(typeof(EventPage));
                }
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
