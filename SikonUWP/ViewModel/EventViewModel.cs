using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ModelLibrary.Exceptions;
using ModelLibrary.Model;
using SikonUWP.Annotations;
using SikonUWP.Common;
using SikonUWP.Model;
using SikonUWP.View;

namespace SikonUWP.ViewModel
{
    public class EventViewModel : INotifyPropertyChanged
    {
        public MainViewModel ViewModel { get; set; }
        public EventSingleton EventSing { get; set; }
        public RegistrationSingleton RegiSing { get; set; }

        public Event ShownEvent { get; set; }
        public ObservableCollection<Participant> Participants { get; set; }

        #region ControlProperties

        public bool StatusBoxOpen { get; set; }

        public string AdminVisible { get; set; }

        public int RemainingSpots => ShownEvent.MaxNoParticipant - Participants.Count;
        public int TakenSpots => Participants.Count;

        public string SubText { get; set; }
        public string SubColor { get; set; }
        public string SubColor2 { get; set; }
        public bool IsSubed { get; set; }

        private bool _isWorking;

        #endregion

        public ICommand SubCommand { get; set; }
        public ICommand OpenStatusBoxCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand UnSubParticipantCommand { get; set; }

        public EventViewModel()
        {
            ViewModel = MainViewModel.Instance;
            EventSing = EventSingleton.Instance;
            RegiSing = RegistrationSingleton.Instance;

            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                ShownEvent = EventSing.ViewedEvent;
                Participants = RegiSing.RegistrationDictionary[ShownEvent];
                if (ViewModel.IsParticipant != null && Participants.Contains(ViewModel.IsParticipant))
                    IsSubed = true;
                if (ViewModel.IsAdmin != null || ViewModel.IsSpeaker == ShownEvent.Speaker)
                    AdminVisible = "Visible";
                else
                    AdminVisible = "Collapsed";
            }
            else
                ShownEvent = new Event(0, "En lang title på 100 karaktere er en ret lang title. Den er til for at teste maksimalerne. Sådan sea", "Efterhånden er det ved at blive mere alment kendt, at autisme/Aspergers Syndrom ikke er en 'fabriksfejl', men en anden måde at fungere hjernemæssigt på. Denne måde betyder, at man fra dag 1 er bygget til i mindre grad at fornemme sig vej gennem livet, og i større grad at måtte forstå alt. Dette dræner én mere for kræfter, gør én mere socialt udsat, og ofte får andre menneskers handlinger og motiver til at fremstå ulogiske Det betyder også, at vejen til en identitet – en udvikling, alle mennesker gennemgår, og som er svær nok i sig selv – forløber anderledes end for flertallet af befolkningen.Dette handler både om i et fem - fase - forløb at komme ud over den truende fornemmelse af at være 'forkert' og om at bruge andre måder at finde sig selv på end de måder, som giver mening for mange andre – men netop ikke for mennesker på autismespektret.Vejen til identitet med autisme er således kernen i dette indlæg.Efterhånden er det ved at blive mere alment kendt, at autisme / Aspergers Syndrom ikke er en 'fabriksfejl', men en anden måde at fungere hjernemæssigt på.Denne måde betyder, at man fra dag 1 er bygget til i mindre grad at fornemme sig vej gennem livet, og i større grad at måtte forstå alt.Dette dræner én mere for kræfter, gør én mere socialt udsat, og ofte får andre menneskers handlinger og motiver til at fremstå ulogiske Det betyder også, at vejen til en identitet – en udvikling, alle mennesker gennemgår, og som er svær nok i sig selv – forløber anderledes end for flertallet af befolkningen.Dette handler både om i et fem - fase - forløb at komme ud over den truende fornemmelse af at være 'forkert' og om at bruge andre måder at finde sig selv på end de måder, som giver mening for mange andre – men netop ikke for mennesker på autismespektret.Vejen til identitet med autisme er således kernen i dette indlæg Efterhånden er det ved at blive mere alment kendt, at autisme / Aspergers Syndrom ikke er en 'fabriksfejl', men en anden måde at fungere hjernemæssigt på.Denne måde betyder, at man fra dag 1 er bygget til i mindre grad at fornemme sig vej gennem livet, og i større grad at måtte forstå alt.Dette dræner én mere for kræfter, gør én mere socialt udsat, og ofte får andre menneskers handlinger og motiver til at fremstå ulogiske Det betyder også, at vejen til en identitet – en udvikling, alle mennesker gennemgår, og som er svær nok i sig selv – forløber anderledes end for flertallet af befolkningen.Dette handler både om i et fem - fase - forløb at komme ud over den truende fornemmelse af at være 'forkert' og om at bruge andre måder at finde sig selv på end de måder, som giver mening for mange andre – men netop ikke for mennesker på autismespektret.Vejen til identitet med autisme er således kernen i dette indlæg Efterhånden er det ved at blive mere alment kendt, at autisme / Aspergers Syndrom ikke er en 'fabriksfejl', men en anden måde at fungere hjernemæssigt på.Denne måde betyder, at man fra dag 1 er bygget til i mindre grad at fornemme sig ve", Event.EventType.Konkurrence, Event.EventSubject.PædagogiskUdvikling, 110, DateTimeOffset.Now.AddDays(1), DateTimeOffset.Now.AddHours(26), null,null, "Stuff");

            ChangeSubButton();
            
            SubCommand = new RelayCommand(Subscribe, CanSubscribe);
            OpenStatusBoxCommand = new RelayCommand(() => { StatusBoxOpen = true; OnPropertyChanged(nameof(StatusBoxOpen)); });
            UpdateCommand = new RelayCommand(Update);
            DeleteCommand = new RelayCommand(Delete);
            ClearCommand = new RelayCommand(Clear);
            UnSubParticipantCommand = new RelayCommand(UnSubParticipant);
        }

        #region CommandMethods

        public bool CanSubscribe()
        {
            return ViewModel.IsParticipant != null && ShownEvent.MaxNoParticipant > Participants.Count;
        }

        public async void Subscribe()
        {
            if (!_isWorking && ShownEvent.MaxNoParticipant > Participants.Count)
            {
                _isWorking = true;
                if (IsSubed)
                    await RegiSing.RemoveRegistration(ShownEvent, ViewModel.IsParticipant);
                else
                    await RegiSing.AddRegistration(ShownEvent, ViewModel.IsParticipant);
                
                IsSubed = !IsSubed;
                ChangeSubButton();
                OnPropertyChanged(nameof(RemainingSpots));
                OnPropertyChanged(nameof(TakenSpots));
                _isWorking = false;
            }
        }

        public void ChangeSubButton()
        {
            if (IsSubed)
            {
                SubText = "Afmeld";
                SubColor = "Gray";
                SubColor2 = "DarkGray";
            }
            else
            {
                SubText = "Tilmeld";
                SubColor = "#FFD87A00";
                SubColor2 = "#FFA95F00";
            }
            OnPropertyChanged(nameof(SubText));
            OnPropertyChanged(nameof(SubColor));
            OnPropertyChanged(nameof(SubColor2));
        }

        public void Update()
        {
            EventSing.MarkedEvent = ShownEvent;
            ViewModel.NavigateToPage(typeof(EventEditorPage));
        }

        public async void Delete()
        {
            bool ok = await MessageDialogUtil.InputDialogAsync("Er du sikker?", "Er du sikker på at du vil slette denne begivenhed fuldstændig");
            if (ok)
            {
                ViewModel.LoadText = "Sletter begivenheden";
                await RegiSing.DeleteEvent(ShownEvent);
                ok = await EventSing.EventCatalog.Remove(ShownEvent.Id);
                ok = ok && await ImageSingleton.Instance.ImageCatalog.RemoveImage(ShownEvent.ImageName);
                if (!ok)
                {
                    ViewModel.LoadText = "Fejl";
                    throw new BaseException("Failed to delete event");
                }
                ViewModel.LoadText = null;
                EventSing.ViewedEvent = null;
                MainViewModel.Instance.NavigateToPage(typeof(EventHomePage));
            }
        }

        public async void Clear()
        {
            bool ok = await MessageDialogUtil.InputDialogAsync("Er du sikker?", "Er du sikker på at du vil afmelde alle deltagere fra denne begivenhed");
            if (ok)
                await RegiSing.ClearRegistration(ShownEvent);
            IsSubed = false;
            ChangeSubButton();
            OnPropertyChanged(nameof(RemainingSpots));
            OnPropertyChanged(nameof(TakenSpots));
        }

        public async void UnSubParticipant(object parameter)
        {
            await RegiSing.RemoveRegistration(ShownEvent, (Participant)parameter);
            IsSubed = Participants.Contains(ViewModel.IsParticipant);
            ChangeSubButton();
            OnPropertyChanged(nameof(RemainingSpots));
            OnPropertyChanged(nameof(TakenSpots));
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
