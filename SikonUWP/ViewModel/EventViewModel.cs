using System;
using System.Collections.Generic;
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
        public EventSingleton EventSing { get; set; }

        public Event ShownEvent { get; set; }

        #region ControlProperties

        public bool StatusBoxOpen { get; set; }

        #endregion

        public ICommand SubCommand { get; set; }
        public ICommand UnSubCommand { get; set; }
        public ICommand OpenStatusBox { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand ClearCommand { get; set; }

        public EventViewModel()
        {
            EventSing = EventSingleton.Instance;

            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                ShownEvent = EventSing.ViewedEvent;
            }
            else
                ShownEvent = new Event(0, "En lang title på 100 karaktere er en ret lang title. Den er til for at teste maksimalerne. Sådan sea", "Efterhånden er det ved at blive mere alment kendt, at autisme/Aspergers Syndrom ikke er en 'fabriksfejl', men en anden måde at fungere hjernemæssigt på. Denne måde betyder, at man fra dag 1 er bygget til i mindre grad at fornemme sig vej gennem livet, og i større grad at måtte forstå alt. Dette dræner én mere for kræfter, gør én mere socialt udsat, og ofte får andre menneskers handlinger og motiver til at fremstå ulogiske Det betyder også, at vejen til en identitet – en udvikling, alle mennesker gennemgår, og som er svær nok i sig selv – forløber anderledes end for flertallet af befolkningen.Dette handler både om i et fem - fase - forløb at komme ud over den truende fornemmelse af at være 'forkert' og om at bruge andre måder at finde sig selv på end de måder, som giver mening for mange andre – men netop ikke for mennesker på autismespektret.Vejen til identitet med autisme er således kernen i dette indlæg.Efterhånden er det ved at blive mere alment kendt, at autisme / Aspergers Syndrom ikke er en 'fabriksfejl', men en anden måde at fungere hjernemæssigt på.Denne måde betyder, at man fra dag 1 er bygget til i mindre grad at fornemme sig vej gennem livet, og i større grad at måtte forstå alt.Dette dræner én mere for kræfter, gør én mere socialt udsat, og ofte får andre menneskers handlinger og motiver til at fremstå ulogiske Det betyder også, at vejen til en identitet – en udvikling, alle mennesker gennemgår, og som er svær nok i sig selv – forløber anderledes end for flertallet af befolkningen.Dette handler både om i et fem - fase - forløb at komme ud over den truende fornemmelse af at være 'forkert' og om at bruge andre måder at finde sig selv på end de måder, som giver mening for mange andre – men netop ikke for mennesker på autismespektret.Vejen til identitet med autisme er således kernen i dette indlæg Efterhånden er det ved at blive mere alment kendt, at autisme / Aspergers Syndrom ikke er en 'fabriksfejl', men en anden måde at fungere hjernemæssigt på.Denne måde betyder, at man fra dag 1 er bygget til i mindre grad at fornemme sig vej gennem livet, og i større grad at måtte forstå alt.Dette dræner én mere for kræfter, gør én mere socialt udsat, og ofte får andre menneskers handlinger og motiver til at fremstå ulogiske Det betyder også, at vejen til en identitet – en udvikling, alle mennesker gennemgår, og som er svær nok i sig selv – forløber anderledes end for flertallet af befolkningen.Dette handler både om i et fem - fase - forløb at komme ud over den truende fornemmelse af at være 'forkert' og om at bruge andre måder at finde sig selv på end de måder, som giver mening for mange andre – men netop ikke for mennesker på autismespektret.Vejen til identitet med autisme er således kernen i dette indlæg Efterhånden er det ved at blive mere alment kendt, at autisme / Aspergers Syndrom ikke er en 'fabriksfejl', men en anden måde at fungere hjernemæssigt på.Denne måde betyder, at man fra dag 1 er bygget til i mindre grad at fornemme sig ve", Event.EventType.Konkurrence, Event.EventSubject.PædagogiskUdvikling, 110, DateTimeOffset.Now.AddDays(1), DateTimeOffset.Now.AddHours(26), null,null, "Stuff");
            
            SubCommand = new RelayCommand(Subscribe);
            UnSubCommand = new RelayCommand(UnSubscribe);
            OpenStatusBox = new RelayCommand(() => { StatusBoxOpen = true; OnPropertyChanged(nameof(StatusBoxOpen)); });
            UpdateCommand = new RelayCommand(Update);
            DeleteCommand = new RelayCommand(Delete);
            ClearCommand = new RelayCommand(Clear);
        }

        #region CommandMethods

        public void Subscribe()
        {

        }

        public void UnSubscribe()
        {

        }

        public void Update()
        {
            EventSing.MarkedEvent = ShownEvent;
            MainViewModel.Instance.NavigateToPage(typeof(EventEditorPage));
        }

        public async void Delete()
        {
            bool ok = await MessageDialogUtil.InputDialogAsync("Er du sikker?", "Er du sikker på at du vil slette denne begivenhed fuldstændig");
            if (ok)
            {
                MainViewModel mainViewModel = MainViewModel.Instance;
                mainViewModel.LoadText = "Sletter begivenheden";
                ok = await EventSing.EventCatalog.Remove(ShownEvent.Id);
                ok = ok && await ImageSingleton.Instance.ImageCatalog.RemoveImage(ShownEvent.ImageName);
                if (!ok)
                {
                    mainViewModel.LoadText = "Fejl";
                    throw new BaseException("Failed to delete event");
                }
                mainViewModel.LoadText = null;
            }
        }

        public void Clear()
        {

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
