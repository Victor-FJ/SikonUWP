using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using ModelLibrary.Exceptions;
using SikonUWP.Persistency;
using ModelLibrary.Model;
using SikonUWP.Annotations;
using SikonUWP.Common;
using SikonUWP.Handlers;
using SikonUWP.Model;
using SikonUWP.View;

namespace SikonUWP.ViewModel
{
    public class EventEditorViewModel : INotifyPropertyChanged
    {
        public EventSingleton EventSing { get; set; }

        public Event EditedEvent { get; set; }

        #region ControlProperties

        //TooltipAndCom properties for color and text for the tooltips and control borders

        private readonly string[] _toolTipText =
        {
            "Begivenhedens title tekst", "Den person som er vært for denne begivenheden",
            "Den dato hvor begivenheden afholdes", "Det tidspunkt hvor begivenheden begynder",
            "Det tidspunkt hvor begivenheden slutter", "En beskrivelse af begivenheden",
            "Hvilken form for begivenhed der er tale om", "Hvilket emne som begivenheden handler om",
            "Det lokale hvor begivenheden forgår", "Det antal personer begivenheden højst kan have",
            "Et billed til begivenheden"
        };

        private const string ColorRed = "Red";
        private const string ColorGray = "DimGray";
        private readonly string[] _toolTipColor = Enumerable.Repeat(ColorGray, 11).ToArray();

        public string[] ToolTipText { get; private set; }

        public string[] ToolTipColor { get; private set; }


        //Title property

        public string Title
        {
            get => EventSing.MarkedBools[0] ? EditedEvent.Title : null;
            set
            {
                try
                {
                    EditedEvent.Title = value;
                    ToolTip(0);
                    EventSing.MarkedBools[0] = true;
                }
                catch (EmptyException ex)
                {
                    ToolTip(0, ex.Message);
                }
            }
        }


        //Description property

        public string Description
        {
            get => EventSing.MarkedBools[1] ? EditedEvent.Description : null;
            set
            {
                try
                {
                    EditedEvent.Description = value;
                    TooltipAndCom(5, 1);
                    EventSing.MarkedBools[1] = true;
                }
                catch (EmptyException ex)
                {
                    ToolTip(5, ex.Message);
                }
            }
        }


        //Type and Subject properties containing the list of enums (without none) and the selected

        public List<Event.EventType> EventTypes => Enum.GetValues(typeof(Event.EventType)).OfType<Event.EventType>().ToList();
        public Event.EventType? SelectedType
        {
            get => EventSing.MarkedBools[2] ? (Event.EventType?)EditedEvent.Type : null;
            set
            {
                if (value != null) EditedEvent.Type = value.Value;
            }
        }

        public List<Event.EventSubject> EventSubjects => Enum.GetValues(typeof(Event.EventSubject)).OfType<Event.EventSubject>().ToList();
        public Event.EventSubject? SelectedSubject
        {
            get => EventSing.MarkedBools[3] ? (Event.EventSubject?)EditedEvent.Type : null;
            set
            {
                if (value != null) EditedEvent.Subject = value.Value;
            }
        }

        //MaxNoParticipant

        public int MaxNoParticipant
        {
            get => EventSing.MarkedBools[4] ? EditedEvent.MaxNoParticipant : 0;
            set
            {
                try
                {
                    EditedEvent.MaxNoParticipant = value;
                    TooltipAndCom(9,4);
                }
                catch (OutsideRangeException ex)
                {
                    ToolTip(9, ex.Message);
                }
            }
        }

        //Date properties for checking that dates do not conflict

        private DateTimeOffset _startDate;
        private DateTimeOffset _endDate;

        private DateTimeOffset? _date;
        public DateTimeOffset? Date
        {
            get => _date;
            set
            {
                if (value != null)
                {
                    _date = new DateTimeOffset(value.Value.Date, value.Value.Offset);
                    if (_date < DateTimeOffset.Now.Date)
                        ToolTip(5, "Datoen kan ikke ligge i fortiden");
                    else
                        SetDateTime();
                }
            }
        }

        private TimeSpan? _startTime;
        public TimeSpan? StartTime
        {
            get => _startTime;
            set
            {
                _startTime = value == TimeSpan.Zero ? null : value;
                if (_startTime != null)
                    SetDateTime();
            }
        }

        private TimeSpan? _endTime;
        public TimeSpan? EndTime
        {
            get => _endTime;
            set
            {
                _endTime = value == TimeSpan.Zero ? null : value;
                if (_endTime != null)
                    SetDateTime();
            }
        }


        //Room properties
        private const string _message = "Max antal deltagere er højere en rummet kan holde";

        private readonly List<Room> _rooms = new List<Room>()
        {
            new Room("A4.24", "Op af trappen og til venstre, der vil den ligge på højre side", 20),
            new Room("A1.01", "You cant miss it", 110),
            new Room("B2.11", "Some closet on the right", 5)
        };
        public List<Room> Rooms => _rooms;

        public Room SelectedRoom
        {
            get => EventSing.MarkedBools[7] ? EditedEvent.Room : null;
            set
            {
                int number = EditedEvent.MaxNoParticipant;
                EditedEvent.Room = value;
                try
                {
                    EditedEvent.MaxNoParticipant = number;
                    ToolTip(4);
                }
                catch (OutsideRangeException ex)
                {
                    ToolTip(4, ex.Message);
                }
            }
            //TODO: When seting a new room perform date check again
        }


        //Speaker properties

        private readonly List<Speaker> _speakers = new List<Speaker>()
        {
            new Speaker("Victor", "2109", "Victor Friis-Jensen", "Jeg er en glad ung gut"),
            new Speaker("Nicolai", "1234", "Nicolai Höyer Christiansen", "Endnu en gut"),
            new Speaker("Sebastian", "hmmm", "Sebastian Halkjær Petersen", "Så mange gutter")
        };
        public List<Speaker> Speakers => _speakers;

        public Speaker SelectedSpeaker
        {
            get => EventSing.MarkedBools[8] ? EditedEvent.Speaker : null;
            set => EditedEvent.Speaker = value;
            //TODO: When seting a new speaker perform date check again
        }

        private readonly List<Event> _possConEvents = new List<Event>() { new Event(0, "Test event", "Test", Event.EventType.Tema, Event.EventSubject.Autisme, 10, DateTimeOffset.Now.AddDays(2), DateTimeOffset.Now.AddHours(50), null, null, "SomeImage") };

        
        //ImageView

        private object _imageView;
        public object ImageView
        {
            get => _imageView;
            set
            {
                _imageView = value;
                OnPropertyChanged();
            }
        }


        #endregion

        public ICommand GetImageCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand CreateCommand { get; set; }

        public EventEditorViewModel()
        {
            EventSing = EventSingleton.Instance;
            //EventSing.MarkedEvent = new Event(0, "Test event", "Test", Event.EventType.Tema, Event.EventSubject.Autisme, 10,
            //    DateTimeOffset.Now.AddDays(2), DateTimeOffset.Now.AddHours(50), Rooms[0], Speakers[0], "SomeImage");

            if (EventSing.MarkedEvent == null)
                EventSing.MarkedEvent = new Event();
            EditedEvent = EventSing.MarkedEvent;
            
            StartUpToolTip();
            StartUpDate();

            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                StartUpImage();

            GetImageCommand = new RelayCommand(async () => {
                EventSing.MarkedImage = await ImageHandler.PickSingleImage();
                ImageView = await ImageHandler.AsBitmapImage(EventSing.MarkedImage);
            });
            ClearCommand = new RelayCommand(Clear);

        }

        #region StartUpMethods

        private void StartUpToolTip()
        {
            ToolTipText = (string[])_toolTipText.Clone();
            ToolTipColor = _toolTipColor;
        }

        private void StartUpDate()
        {
            _date = EditedEvent.StartDate == DateTimeOffset.MinValue ? (DateTimeOffset?)null :
                new DateTimeOffset(EditedEvent.StartDate.Date, EditedEvent.StartDate.Offset);
            _startTime = EditedEvent.StartDate.TimeOfDay;
            if (_startTime == TimeSpan.Zero) _startTime = null;
            _endTime = EditedEvent.EndDate.TimeOfDay;
            if (_endTime == TimeSpan.Zero) _endTime = null;
        }

        private async void StartUpImage()
        {
            if (EventSing.MarkedImage != null)
                ImageView = await ImageHandler.AsBitmapImage(EventSing.MarkedImage);
            if (ImageHandler.Dictionary != null && EditedEvent.ImageName != null && ImageHandler.Dictionary.ContainsKey(EditedEvent.ImageName))
                ImageView = EditedEvent.ImageName;
        }

        #endregion

        #region CommandMethods

        public async void Clear()
        {
            if (await MessageDialogUtil.InputDialogAsync("Er du sikker?",
                "Er du sikker på at du vil ryde indtastet begivenheds info og starte forfra blankt?"))
            {
                EventSing.MarkedEvent = new Event();
                EventSing.MarkedImage = null;
                MainViewModel.Instance.NavigateToPage(typeof(EventEditorPage));
            }
        }

        #endregion

        #region ViewMethods

        private void TooltipAndCom(int index, int completed)
        {
            EventSing.MarkedBools[completed] = true;
            ToolTip(index);
        }

        private void ToolTip(int index) => ToolTip(index, _toolTipText[index], 0);
        private void ToolTip(int index, string text) => ToolTip(index, text, 1);

        private void ToolTip(int index, string text, int color)
        {
            ToolTipText[index] = text;
            ToolTipColor[index] = color == 1 ? ColorRed : ColorGray;
            OnPropertyChanged(nameof(ToolTipText));
            OnPropertyChanged(nameof(ToolTipColor));
        }

        private async void SetDateTime()
        {
            ToolTip(2);
            TooltipAndCom(3, 5);
            TooltipAndCom(4, 6);

            if (_date != null)
            {
                bool gotStart = _startTime != null;
                bool gotEnd = _endTime != null;
                if (gotStart) _startDate = _date.Value.Add(_startTime.Value);
                if (gotEnd) _endDate = _date.Value.Add(_endTime.Value);
                if (gotStart && gotEnd && _startTime.Value > _endTime.Value)
                {
                    _endDate = _endDate.AddDays(1);
                    await MessageDialogUtil.MessageDialogAsync("Er du sikker?",
                        "Vær opmærksom på at det valgte slut-tidspunkt først rammes dagen efter start-tidspunktet");
                }

                bool overlap = false;
                bool startPo = false;
                bool endPo = false;
                foreach (Event possConEvent in _possConEvents)
                {
                    const string message1 = "Tidsforløbet overlaber en anden begivenhed som værten er tilmeldt";
                    const string message2 = "Tidspunktet ligger oven i en anden begivenhed som værten er tilmeldt";
                    if (gotStart && gotEnd && !overlap && _startDate < possConEvent.EndDate && _endDate > possConEvent.StartDate)
                    {
                        ToolTip(3, message1);
                        ToolTip(4, message1);
                        overlap = true;
                    }

                    if (gotStart && !startPo && _startDate > possConEvent.StartDate && _startDate < possConEvent.EndDate)
                    {
                        ToolTip(3, message2);
                        startPo = true;
                    }

                    if (gotEnd && !endPo && _endDate > possConEvent.StartDate && _endDate < possConEvent.EndDate)
                    {
                        ToolTip(4, message2);
                        endPo = true;
                    }

                    if (overlap && startPo && endPo)
                        return;
                }

                if (overlap || startPo || endPo)
                    return;
                EditedEvent.StartDate = _startDate;
                EditedEvent.EndDate = _endDate;
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
