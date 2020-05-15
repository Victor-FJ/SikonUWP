using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ModelLibrary.Model;
using SikonUWP.Annotations;
using SikonUWP.Model;

namespace SikonUWP.ViewModel
{
    class EventScheduleViewModel: INotifyPropertyChanged
    {

        private EventSingleton Singleton { get; set; }

        public ReadOnlyCollection<Event> Collection => SortCollection();

        private ReadOnlyCollection<Event> SortCollection()
        {
            return (from @event in EventSingleton.Instance.EventCatalog.Collection 
                orderby @event.StartDate select @event).ToList().AsReadOnly();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
    }
}
