﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using ModelLibrary.Exceptions;
using SikonUWP.Persistency;
using ModelLibrary.Model;
using SikonUWP.Annotations;
using SikonUWP.Common;
using SikonUWP.Model;
using SikonUWP.View;

namespace SikonUWP.ViewModel
{
    public class EventEditorViewModel : INotifyPropertyChanged
    {
        public EventSingleton EventSing { get; set; }
        public ImageSingleton ImageSing { get; set; }
        public RegistrationSingleton RegiSing { get; set; }

        public MainViewModel MainViewModel { get; set; }

        public Event EditedEvent { get; set; }

        #region ControlProperties

        //ToolTip properties for color and text for the tooltips and control borders

        private readonly string[] _toolTipText =
        {
            "Begivenhedens title tekst", "En beskrivelse af begivenheden",
            "Hvilken form for begivenhed der er tale om", "Hvilket emne som begivenheden handler om",
            "Det antal personer begivenheden højst kan have", "Den dato hvor begivenheden afholdes", 
            "Det tidspunkt hvor begivenheden begynder", "Det tidspunkt hvor begivenheden slutter",
            "Det lokale hvor begivenheden forgår", "Den person som er vært for denne begivenheden",
            "Et billed til begivenheden", "Navnet til billedet", "Dato ting"
        };

        private const string ColorRed = "Red";
        private const string ColorGray = "DimGray";
        private readonly string[] _toolTipColor = Enumerable.Repeat(ColorGray, 13).ToArray();

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
                    ToolTip(1);
                }
                catch (EmptyException ex)
                {
                    ToolTip(1, ex.Message);
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
                ToolTip(2);
            }
        }

        public List<Event.EventSubject> EventSubjects => Enum.GetValues(typeof(Event.EventSubject)).OfType<Event.EventSubject>().ToList();
        public Event.EventSubject? SelectedSubject
        {
            get => EventSing.MarkedBools[3] ? (Event.EventSubject?)EditedEvent.Subject : null;
            set
            {
                if (value != null) EditedEvent.Subject = value.Value;
                ToolTip(3);
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
                    ToolTip(4);
                }
                catch (OutsideRangeException ex)
                {
                    ToolTip(4, ex.Message);
                }
            }
        }

        //Date properties for checking that dates do not conflict

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
                    {
                        EditedEvent.StartDate = _date.Value;
                        ToolTip(5);
                        SetDateTime();
                    }
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
        public ObservableCollection<Room> Rooms => RoomCatalogSingleton.Instance.Rooms;

        public Room SelectedRoom
        {
            get => EventSing.MarkedBools[8] ? EditedEvent.Room : null;
            set
            {
                int number = EditedEvent.MaxNoParticipant;
                EditedEvent.Room = value;
                ToolTip(8);
                try
                {
                    EditedEvent.MaxNoParticipant = number;
                    ToolTip(4);
                }
                catch (OutsideRangeException ex)
                {
                    ToolTip(4, ex.Message);
                }
                SetDateTime();
            }
        }


        //Speaker properties

        public ObservableCollection<Speaker> Speakers => SpeakerCatalogSingleton.Instance.Speakers;

        public Speaker SelectedSpeaker
        {
            get => EventSing.MarkedBools[9] ? EditedEvent.Speaker : null;
            set
            {
                EditedEvent.Speaker = value;
                ToolTip(9);
                SetDateTime();
            }
        }


        //ImageView

        private BitmapImage _imageView;
        public BitmapImage ImageView
        {
            get => _imageView;
            set
            {
                _imageView = value;
                OnPropertyChanged();
            }
        }


        private bool _editImage;
        public bool EditImage
        {
            get => _editImage;
            set
            {
                _editImage = value;
                OnPropertyChanged();
            }
        }

        private string _oldImageName;

        private string _imageName;
        public string ImageName
        {
            get => EventSing.MarkedBools[11] ? EditedEvent.ImageName : _imageName;
            set
            {
                try
                {
                    EditedEvent.ImageName = value;
                    _imageName = value;
                    if (EventSing.MarkedImage != null)
                        EventSing.EventCatalog.CheckImage(EditedEvent, true);
                    ToolTip(11);
                }
                catch (EmptyException ex)
                {
                    ToolTip(11, ex.Message);
                }
                catch (ItIsNotUniqueException ex)
                {
                    ToolTip(11, ex.Message);
                }
                OnPropertyChanged();
            }
        }


        public string EditButtonText { get; set; }


        private bool _editing;

        #endregion

        public ICommand GetImageCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand EditEventCommand { get; set; }

        public EventEditorViewModel()
        {
            //Gets singletons
            EventSing = EventSingleton.Instance;
            ImageSing = ImageSingleton.Instance;
            RegiSing = RegistrationSingleton.Instance;
            MainViewModel = MainViewModel.Instance;

            //Gets the event marked by previous pages as the event to edit
            EditedEvent = EventSing.MarkedEvent;

            //Start ups
            StartUpToolTip();
            StartUpDate();
            //Start ups not done by designer
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                StartUpImage();
            }
                
            //Commands
            GetImageCommand = new RelayCommand(GetImage);
            ClearCommand = new RelayCommand(Clear);

            //Checks if the marked event is new or not to determine wether to create or update
            if (EventSing.IsNew)
            {
                EditEventCommand = new RelayCommand(() => Edit(Create), () => !_editing);
                EditButtonText = "Skab Begivenheden";
            }
            else
            {
                EditEventCommand = new RelayCommand(() => Edit(Update), () => !_editing);
                _oldImageName = EditedEvent.ImageName;
                EditButtonText = "Opdater Begivenheden";
            }
                
        }

        #region StartUpMethods

        private void StartUpToolTip()
        {
            ToolTipText = (string[])_toolTipText.Clone();
            ToolTipColor = _toolTipColor;
        }

        private void StartUpDate()
        {
            _date = EventSing.MarkedBools[5] ? new DateTimeOffset(EditedEvent.StartDate.Date, EditedEvent.StartDate.Offset) : (DateTimeOffset?)null;
            _startTime = EventSing.MarkedBools[6] ? EditedEvent.StartDate.TimeOfDay : (TimeSpan?)null;
            _endTime = EventSing.MarkedBools[7] ? EditedEvent.EndDate.TimeOfDay : (TimeSpan?)null;
            SetDateTime();
        }

        private async void StartUpImage()
        {
            if (EventSing.MarkedImage != null)
            {
                EditImage = true;
                ImageView = await ImageSing.ImageCatalog.AsBitmapImage(EventSing.MarkedImage);
                ImageName = EditedEvent.ImageName;
            }
            else if (!EventSing.IsNew)
            {
                ImageView = ImageSing.ImageCatalog.Dictionary[EditedEvent.ImageName];
                ImageName = EditedEvent.ImageName;
            }
        }

        #endregion

        #region CommandMethods

        public async void GetImage()
        {
            StorageFile image = await ImageSing.ImageCatalog.PickSingleImage();
            if (image != null)
            {
                EventSing.MarkedImage = image;
                EditImage = true;
                ImageView = await ImageSing.ImageCatalog.AsBitmapImage(EventSing.MarkedImage);
                ImageName = image.Name;
                ToolTip(10);
            }
            else
                await MessageDialogUtil.MessageDialogAsync("Forkert filtype", "Kunne ikke hente billedet");
        }

        public async void Clear()
        {
            if (await MessageDialogUtil.InputDialogAsync("Er du sikker?",
                "Er du sikker på at du vil ryde indtastet begivenheds info og starte forfra blankt?"))
            {
                EventSing.MarkedEvent = null;
                MainViewModel.Instance.NavigateToPage(typeof(EventEditorPage));
            }
        }

        public async void Edit(Func<Task> method)
        {
            bool[] comBools = EventSing.MarkedBools;
            if (comBools.All(x => x))
            {
                _editing = true;
                ((RelayCommand)EditEventCommand).RaiseCanExecuteChanged();
                try
                {
                    await method.Invoke();
                    EventSing.MarkedEvent = null;
                    EventSing.ViewedEvent = EditedEvent;
                    MainViewModel.NavigateToPage(typeof(EventPage));
                }
                catch (HttpRequestException)
                {
                    await MessageDialogUtil.MessageDialogAsync(PersistencyManager.FileName, PersistencyManager.Message);
                    _editing = false;
                    ((RelayCommand)EditEventCommand).RaiseCanExecuteChanged();
                }
            }
            else
            {
                for (int i = 0; i < comBools.Length; i++)
                    if (!comBools[i])
                        ToolTip(i, "Skal være udfyldt");
                if (!comBools[4])
                    ToolTip(4, "Kan ikke være 0", ColorRed);
            }
        }

        public async Task Create()
        {
            MainViewModel.LoadText = "Skaber begivenhed";
            bool eventOk = false;
            bool imageOk = await ImageSing.ImageCatalog.AddImage(EventSing.MarkedImage, ImageName);
            if (imageOk)
                eventOk = await EventSing.EventCatalog.Add(EditedEvent, true);
            if (eventOk)
            {
                RegiSing.AddEvent(EditedEvent);
                MainViewModel.LoadText = null;
                return;
            }
            MainViewModel.LoadText = "Fejl";
            if (imageOk)
                await ImageSing.ImageCatalog.RemoveImage(ImageName);
            throw new BaseException("You though this would not be possible. Looks like you missed something");
        }

        public async Task Update()
        {
            MainViewModel.LoadText = "Opdatere begivenhed";
            bool eventOk = false;
            bool imageOk;
            if (EventSing.MarkedImage != null)
                imageOk = await ImageSing.ImageCatalog.AddImage(EventSing.MarkedImage, ImageName);
            else
                imageOk = true;
            if (imageOk)
                eventOk = await EventSing.EventCatalog.Update(EditedEvent.Id, EditedEvent);
            if (EventSing.MarkedImage != null && eventOk)
                imageOk = await ImageSing.ImageCatalog.RemoveImage(_oldImageName);
            if (eventOk && imageOk)
            {
                Event oldEvent = RegiSing.RegistrationDictionary.Single(x => x.Key.Id == EditedEvent.Id).Key;
                RegiSing.UpdateEvent(oldEvent, EditedEvent);
                MainViewModel.LoadText = null;
                return;
            }
            MainViewModel.LoadText = "Fejl";
            if (imageOk)
                await ImageSing.ImageCatalog.RemoveImage(ImageName);
            throw new BaseException("You though this would not be possible. Looks like you missed something");
        }

        #endregion

        #region ViewMethods

        private void ToolTip(int index)
        {
            EventSing.MarkedBools[index] = true;
            ToolTip(index, _toolTipText[index], ColorGray);
        }

        private void ToolTip(int index, string text) => ToolTip(index, text, ColorRed);

        private void ToolTip(int index, string text, string color)
        {
            ToolTipText[index] = text;
            ToolTipColor[index] = color;
            OnPropertyChanged(nameof(ToolTipText));
            OnPropertyChanged(nameof(ToolTipColor));
        }

        private async void SetDateTime()
        {
            if (_date != null)
            {
                bool gotStart = _startTime != null;
                bool gotEnd = _endTime != null;
                if (gotStart)
                {
                    EditedEvent.StartDate = _date.Value.Add(_startTime.Value);
                    ToolTip(6);
                }
                if (gotEnd)
                    try
                    {
                        EditedEvent.EndDate = _date.Value.Add(_endTime.Value);
                        ToolTip(7);
                    }
                    catch (OutsideRangeException)
                    {
                        DateTimeOffset endDate = _date.Value.Add(_endTime.Value);
                        EditedEvent.EndDate = endDate.AddDays(1);
                        ToolTip(7);
                        await MessageDialogUtil.MessageDialogAsync("Er du sikker?",
                            "Vær opmærksom på at det valgte slut-tidspunkt først rammes dagen efter start-tidspunktet");
                    }

                if (gotStart && gotEnd)
                    try
                    {
                        EventSing.EventCatalog.CheckDate(EditedEvent);
                        EventSing.MarkedBools[12] = true;
                    }
                    catch (OutsideRangeException ex)
                    {
                        ToolTip(6, ex.Message);
                        ToolTip(7, ex.Message);
                        EventSing.MarkedBools[12] = false;
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
