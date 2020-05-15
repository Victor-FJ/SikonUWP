using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using ModelLibrary.Model;
using SikonUWP.Annotations;
using SikonUWP.Common;
using SikonUWP.Model;
using SikonUWP.View;

namespace SikonUWP.ViewModel
{
    public class EventHomeViewModel : INotifyPropertyChanged
    {
        public EventSingleton EventSing { get; set; }

        private IEnumerable<Event> _events;
        public ObservableCollection<Event> Events { get; set; }

        #region ControlProperties

        private readonly ReadOnlyCollection<string> _orderList = new List<string>()
        {
            "Id",
            "Title",
            "Oplægsholder",
            "Dato"
        }.AsReadOnly();

        public ReadOnlyCollection<string> OrderList => _orderList;

        private string _selectedOrder;
        public string SelectedOrder
        {
            get { return _selectedOrder; }
            set
            {
                _selectedOrder = value;
                SortEvents();
            }
        }


        private const string TypeText = "Alle typer";
        public ReadOnlyCollection<string> Types => EnumList<Event.EventType>(TypeText);

        private string _selectedType;
        public string SelectedType
        {
            get { return _selectedType; }
            set
            {
                _selectedType = value;
                FilterEvents();
            }
        }


        private const string SubjectText = "Alle emner";
        public ReadOnlyCollection<string> Subjects => EnumList<Event.EventSubject>(SubjectText);

        private string _selectedSubject;
        public string SelectedSubject
        {
            get { return _selectedSubject; }
            set
            {
                _selectedSubject = value;
                FilterEvents();
            }
        }


        private readonly Speaker _speakerText = new Speaker("dummySpeaker", "1", "Alle oplægsholdere", "f", "f");

        private ReadOnlyCollection<Speaker> _speakers;
        public ReadOnlyCollection<Speaker> Speakers => _speakers;

        private Speaker _selectedSpeaker;
        public Speaker SelectedSpeaker
        {
            get { return _selectedSpeaker; }
            set
            {
                _selectedSpeaker = value;
                FilterEvents();
            }
        }


        private DateTimeOffset? _selectedDate;

        public DateTimeOffset? SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                _selectedDate = value;
                FilterEvents();
            }
        }



        #endregion

        public ICommand NavigateToEventCommand { get; set; }
        public ICommand ShowAllCommand { get; set; }

        public EventHomeViewModel()
        {
            EventSing = EventSingleton.Instance;

            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                Load();
            else
            {
                SpeakerList();
                Events = new ObservableCollection<Event>();
                Events.Add(new Event(0, "En lang title på 100 karaktere er en ret lang title. Den er til for at teste maksimalerne. Sådan sea", "Efterhånden er det ved at blive mere alment kendt, at autisme/Aspergers Syndrom ikke er en 'fabriksfejl', men en anden måde at fungere hjernemæssigt på. Denne måde betyder, at man fra dag 1 er bygget til i mindre grad at fornemme sig vej gennem livet, og i større grad at måtte forstå alt. Dette dræner én mere for kræfter, gør én mere socialt udsat, og ofte får andre menneskers handlinger og motiver til at fremstå ulogiske Det betyder også, at vejen til en identitet – en udvikling, alle mennesker gennemgår, og som er svær nok i sig selv – forløber anderledes end for flertallet af befolkningen.Dette handler både om i et fem - fase - forløb at komme ud over den truende fornemmelse af at være 'forkert' og om at bruge andre måder at finde sig selv på end de måder, som giver mening for mange andre – men netop ikke for mennesker på autismespektret.Vejen til identitet med autisme er således kernen i dette indlæg.Efterhånden er det ved at blive mere alment kendt, at autisme / Aspergers Syndrom ikke er en 'fabriksfejl', men en anden måde at fungere hjernemæssigt på.Denne måde betyder, at man fra dag 1 er bygget til i mindre grad at fornemme sig vej gennem livet, og i større grad at måtte forstå alt.Dette dræner én mere for kræfter, gør én mere socialt udsat, og ofte får andre menneskers handlinger og motiver til at fremstå ulogiske Det betyder også, at vejen til en identitet – en udvikling, alle mennesker gennemgår, og som er svær nok i sig selv – forløber anderledes end for flertallet af befolkningen.Dette handler både om i et fem - fase - forløb at komme ud over den truende fornemmelse af at være 'forkert' og om at bruge andre måder at finde sig selv på end de måder, som giver mening for mange andre – men netop ikke for mennesker på autismespektret.Vejen til identitet med autisme er således kernen i dette indlæg Efterhånden er det ved at blive mere alment kendt, at autisme / Aspergers Syndrom ikke er en 'fabriksfejl', men en anden måde at fungere hjernemæssigt på.Denne måde betyder, at man fra dag 1 er bygget til i mindre grad at fornemme sig vej gennem livet, og i større grad at måtte forstå alt.Dette dræner én mere for kræfter, gør én mere socialt udsat, og ofte får andre menneskers handlinger og motiver til at fremstå ulogiske Det betyder også, at vejen til en identitet – en udvikling, alle mennesker gennemgår, og som er svær nok i sig selv – forløber anderledes end for flertallet af befolkningen.Dette handler både om i et fem - fase - forløb at komme ud over den truende fornemmelse af at være 'forkert' og om at bruge andre måder at finde sig selv på end de måder, som giver mening for mange andre – men netop ikke for mennesker på autismespektret.Vejen til identitet med autisme er således kernen i dette indlæg Efterhånden er det ved at blive mere alment kendt, at autisme / Aspergers Syndrom ikke er en 'fabriksfejl', men en anden måde at fungere hjernemæssigt på.Denne måde betyder, at man fra dag 1 er bygget til i mindre grad at fornemme sig ve", Event.EventType.Konkurrence, Event.EventSubject.PædagogiskUdvikling, 110, DateTimeOffset.Now.AddDays(1), DateTimeOffset.Now.AddHours(26), null, null, "Stuff"));
            }


            NavigateToEventCommand = new RelayCommand(NavigateToEvent);
            ShowAllCommand = new RelayCommand(() => { ShowAll(); FilterEvents(); });

            _selectedOrder = _orderList[0];
            ShowAll();
        }

        private async void Load()
        {
            await RoomCatalogSingleton.Instance.LoadRooms();
            await SpeakerCatalogSingleton.Instance.LoadSpeakers();
            SpeakerList();
            await EventSing.EventCatalog.Load();
            SortEvents();
            Events = new ObservableCollection<Event>(EventSing.EventCatalog.Collection);
            OnPropertyChanged(nameof(Events));
        }

        #region CommandMethods

        public void NavigateToEvent(object parameter)
        {
            EventSing.ViewedEvent = (Event)((ItemClickEventArgs)parameter).ClickedItem;
            MainViewModel.Instance.NavigateToPage(typeof(EventPage));
        }

        public void ShowAll()
        {
            _selectedType = TypeText;
            _selectedSubject = SubjectText;
            _selectedSpeaker = _speakerText;
            _selectedDate = null;
            OnPropertyChanged(nameof(SelectedType));
            OnPropertyChanged(nameof(SelectedSubject));
            OnPropertyChanged(nameof(SelectedSpeaker));
            OnPropertyChanged(nameof(SelectedDate));
        }

        #endregion

        #region SortAndFilter

        private void SortEvents()
        {
            if (_selectedOrder == _orderList[0])
                _events = new ObservableCollection<Event>(EventSing.EventCatalog.Collection);
            else if (_selectedOrder == _orderList[1])
                _events = from @event in EventSing.EventCatalog.Collection orderby @event.Title select @event;
            else if (_selectedOrder == _orderList[2])
                _events = from @event in EventSing.EventCatalog.Collection orderby @event.Speaker.FullName select @event;
            else if (_selectedOrder == _orderList[3])
                _events = from @event in EventSing.EventCatalog.Collection orderby @event.StartDate select @event;
            else
                throw new NotImplementedException();

            FilterEvents();
        }

        private void FilterEvents()
        {
            if (Events == null)
                return;

            bool[] filters = new bool[4];

            if (Enum.TryParse(_selectedType, out Event.EventType typeEnum))
                filters[0] = true;
            if (Enum.TryParse(_selectedSubject, out Event.EventSubject subjectEnum))
                filters[1] = true;
            if (_selectedSpeaker != _speakerText)
                filters[2] = true;
            DateTimeOffset selectedDate;
            if (_selectedDate != null)
            {
                filters[3] = true;
                selectedDate = _selectedDate.Value;
            }

            int position = 0;
            foreach (Event @event in _events)
            {
                bool typInc = !filters[0] || filters[0] && @event.Type == typeEnum;
                bool subInc = !filters[1] || filters[1] && @event.Subject == subjectEnum;
                bool speInc = !filters[2] || filters[2] && @event.Speaker == _selectedSpeaker;
                bool datInc = !filters[3] || filters[3] && @event.StartDate.Date == selectedDate.Date;

                bool eveInc = typInc && subInc && speInc && datInc;
                bool eveCon = Events.Contains(@event);
                int eveIndex = Events.IndexOf(@event);

                if (eveInc && !eveCon)
                    Events.Insert(position++, @event);
                else if (eveInc && eveIndex != position)
                    Events.Move(eveIndex, position++);
                else if (eveInc)
                    position++;
                else if (eveCon)
                    Events.Remove(@event);
            }
        }

        #endregion

        #region ComboBoxFilers

        private ReadOnlyCollection<string> EnumList<T>(string allString)
        {
            List<T> enumList = Enum.GetValues(typeof(T)).OfType<T>().ToList();
            List<string> comboList = new List<string>(from @enum in enumList select @enum.ToString());
            comboList.Insert(0, allString);
            return comboList.AsReadOnly();
        }

        private void SpeakerList()
        {
            List<Speaker> speakerList = new List<Speaker>(SpeakerCatalogSingleton.Instance.Speakers);
            speakerList.Insert(0, _speakerText);
            _speakers = speakerList.AsReadOnly();
            OnPropertyChanged(nameof(Speakers));
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
