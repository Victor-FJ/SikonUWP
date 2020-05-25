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
        public RegistrationSingleton RegiSing { get; set; }

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

        public string SelectedOrder
        {
            get { return EventSing.SelOrder; }
            set
            {
                EventSing.SelOrder = value;
                SortEvents();
            }
        }


        private const string TypeText = "Alle typer";
        public ReadOnlyCollection<string> Types => EnumList<Event.EventType>(TypeText);

        public string SelectedType
        {
            get { return EventSing.SelType; }
            set
            {
                EventSing.SelType = value;
                FilterEvents();
            }
        }


        private const string SubjectText = "Alle emner";
        public ReadOnlyCollection<string> Subjects => EnumList<Event.EventSubject>(SubjectText);

        public string SelectedSubject
        {
            get { return EventSing.SelSubject; }
            set
            {
                EventSing.SelSubject = value;
                FilterEvents();
            }
        }


        private const string SpeakerText = "Alle oplægsholdere";

        private readonly ReadOnlyCollection<object> _speakers = SpeakerList();
        public ReadOnlyCollection<object> Speakers => _speakers;

        public object SelectedSpeaker
        {
            get
            {
                if (EventSing.SelSpeaker is string)
                    return Speakers[0];
                if (EventSing.SelSpeaker is Speaker speaker)
                    return Speakers.SingleOrDefault(x => x is Speaker s && s.UserName == speaker.UserName);
                return null;
            }
            set
            {
                EventSing.SelSpeaker = value;
                FilterEvents();
            }
        }


        public DateTimeOffset? SelectedDate
        {
            get { return EventSing.SelDate; }
            set
            {
                EventSing.SelDate = value;
                FilterEvents();
            }
        }

        public int SelectedSpotNo
        {
            get { return EventSing.SelSpotNo; }
            set
            {
                EventSing.SelSpotNo = value;
                FilterEvents();
            }
        }


        private bool _isAllShown;

        #endregion

        public ICommand NavigateToEventCommand { get; set; }
        public ICommand ShowAllCommand { get; set; }

        public EventHomeViewModel()
        {
            EventSing = EventSingleton.Instance;
            RegiSing = RegistrationSingleton.Instance;

            NavigateToEventCommand = new RelayCommand(NavigateToEvent);
            ShowAllCommand = new RelayCommand(() => { ShowAll(); FilterEvents(); }, () => !_isAllShown);

            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                if (EventSing.SelOrder == null)
                {
                    EventSing.SelOrder = _orderList[0];
                    ShowAll();
                }
                StartUp();
            }
        }

        private void StartUp()
        {
            Events = new ObservableCollection<Event>();
            SortEvents();
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
            EventSing.SelType = TypeText;
            EventSing.SelSubject = SubjectText;
            EventSing.SelSpeaker = SpeakerText;
            EventSing.SelDate = null;
            EventSing.SelSpotNo = 0;
            OnPropertyChanged(nameof(SelectedType));
            OnPropertyChanged(nameof(SelectedSubject));
            OnPropertyChanged(nameof(SelectedSpeaker));
            OnPropertyChanged(nameof(SelectedDate));
            OnPropertyChanged(nameof(SelectedSpotNo));
        }

        #endregion

        #region SortAndFilter

        private void SortEvents()
        {
            if (EventSing.SelOrder == _orderList[1])
                _events = from @event in EventSing.EventCatalog.Collection orderby @event.Title select @event;
            else if (EventSing.SelOrder == _orderList[2])
                _events = from @event in EventSing.EventCatalog.Collection orderby @event.Speaker.FullName select @event;
            else if (EventSing.SelOrder == _orderList[3])
                _events = from @event in EventSing.EventCatalog.Collection orderby @event.StartDate select @event;
            else
                _events = new ObservableCollection<Event>(EventSing.EventCatalog.Collection);

            FilterEvents();
        }

        private void FilterEvents()
        {
            bool[] filters = new bool[5];

            if (Enum.TryParse(EventSing.SelType, out Event.EventType typeEnum))
                filters[0] = true;
            if (Enum.TryParse(EventSing.SelSubject, out Event.EventSubject subjectEnum))
                filters[1] = true;
            Speaker speaker = null;
            if (EventSing.SelSpeaker is Speaker s)
            {
                filters[2] = true;
                speaker = s;
            }
                
            DateTimeOffset selectedDate;
            if (EventSing.SelDate != null)
            {
                filters[3] = true;
                selectedDate = EventSing.SelDate.Value;
            }
            if (EventSing.SelSpotNo > 0)
                filters[4] = true;

            int position = 0;
            foreach (Event @event in _events)
            {
                bool typInc = !filters[0] || filters[0] && @event.Type == typeEnum;
                bool subInc = !filters[1] || filters[1] && @event.Subject == subjectEnum;
                bool speInc = !filters[2] || filters[2] && @event.Speaker == speaker && speaker != null;
                bool datInc = !filters[3] || filters[3] && @event.StartDate.Date == selectedDate.Date;
                bool numInc = !filters[4] || filters[4] &&
                    @event.MaxNoParticipant - RegiSing.RegistrationDictionary[@event].Count >=
                    EventSing.SelSpotNo;
                
                bool eveInc = typInc && subInc && speInc && datInc && numInc;
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

            _isAllShown = filters.All(x => !x);
            ((RelayCommand)ShowAllCommand).RaiseCanExecuteChanged();
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

        private static ReadOnlyCollection<object> SpeakerList()
        {
            List<object> speakerList = new List<object>(SpeakerCatalogSingleton.Instance.Speakers);
            speakerList.Insert(0, SpeakerText);
            return speakerList.AsReadOnly();
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
