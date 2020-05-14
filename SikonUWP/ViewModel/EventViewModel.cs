using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ModelLibrary.Exceptions;
using ModelLibrary.Model;
using SikonUWP.Model;

namespace SikonUWP.ViewModel
{
    public class EventViewModel
    {
        public EventSingleton EventSing { get; set; }

        public Event ShownEvent { get; set; }

        public ICommand SubCommand { get; set; }
        public ICommand UnSubCommand { get; set; }

        public EventViewModel()
        {
            EventSing = EventSingleton.Instance;

            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                if (EventSing.IsNew)
                    throw new BaseException("Another error that should not be able to happen");
                ShownEvent = EventSing.MarkedEvent;
            }
            else
                ShownEvent = new Event(0, "En lang title på 100 karaktere er en ret lang title. Den er til for at teste maksimalerne. Sådan sea", "Efterhånden er det ved at blive mere alment kendt, at autisme/Aspergers Syndrom ikke er en 'fabriksfejl', men en anden måde at fungere hjernemæssigt på. Denne måde betyder, at man fra dag 1 er bygget til i mindre grad at fornemme sig vej gennem livet, og i større grad at måtte forstå alt. Dette dræner én mere for kræfter, gør én mere socialt udsat, og ofte får andre menneskers handlinger og motiver til at fremstå ulogiske Det betyder også, at vejen til en identitet – en udvikling, alle mennesker gennemgår, og som er svær nok i sig selv – forløber anderledes end for flertallet af befolkningen.Dette handler både om i et fem - fase - forløb at komme ud over den truende fornemmelse af at være 'forkert' og om at bruge andre måder at finde sig selv på end de måder, som giver mening for mange andre – men netop ikke for mennesker på autismespektret.Vejen til identitet med autisme er således kernen i dette indlæg.Efterhånden er det ved at blive mere alment kendt, at autisme / Aspergers Syndrom ikke er en 'fabriksfejl', men en anden måde at fungere hjernemæssigt på.Denne måde betyder, at man fra dag 1 er bygget til i mindre grad at fornemme sig vej gennem livet, og i større grad at måtte forstå alt.Dette dræner én mere for kræfter, gør én mere socialt udsat, og ofte får andre menneskers handlinger og motiver til at fremstå ulogiske Det betyder også, at vejen til en identitet – en udvikling, alle mennesker gennemgår, og som er svær nok i sig selv – forløber anderledes end for flertallet af befolkningen.Dette handler både om i et fem - fase - forløb at komme ud over den truende fornemmelse af at være 'forkert' og om at bruge andre måder at finde sig selv på end de måder, som giver mening for mange andre – men netop ikke for mennesker på autismespektret.Vejen til identitet med autisme er således kernen i dette indlæg Efterhånden er det ved at blive mere alment kendt, at autisme / Aspergers Syndrom ikke er en 'fabriksfejl', men en anden måde at fungere hjernemæssigt på.Denne måde betyder, at man fra dag 1 er bygget til i mindre grad at fornemme sig vej gennem livet, og i større grad at måtte forstå alt.Dette dræner én mere for kræfter, gør én mere socialt udsat, og ofte får andre menneskers handlinger og motiver til at fremstå ulogiske Det betyder også, at vejen til en identitet – en udvikling, alle mennesker gennemgår, og som er svær nok i sig selv – forløber anderledes end for flertallet af befolkningen.Dette handler både om i et fem - fase - forløb at komme ud over den truende fornemmelse af at være 'forkert' og om at bruge andre måder at finde sig selv på end de måder, som giver mening for mange andre – men netop ikke for mennesker på autismespektret.Vejen til identitet med autisme er således kernen i dette indlæg Efterhånden er det ved at blive mere alment kendt, at autisme / Aspergers Syndrom ikke er en 'fabriksfejl', men en anden måde at fungere hjernemæssigt på.Denne måde betyder, at man fra dag 1 er bygget til i mindre grad at fornemme sig ve", Event.EventType.Konkurrence, Event.EventSubject.PædagogiskUdvikling, 110, DateTimeOffset.Now.AddDays(1), DateTimeOffset.Now.AddHours(26), EventSing.Rooms[1], EventSing.Speakers[0], "Stuff");
        }
    }
}
