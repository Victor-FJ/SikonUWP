using System;
using System.Collections.Generic;
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
    public class EventHomeViewModel : INotifyPropertyChanged
    {
        public Event FirstTestEvent { get; set; }

        public EventHomeViewModel()
        {
            Load();
        }

        private async void Load()
        {
            bool ok = await EventSingleton.Instance.EventCatalog.Load();
            if (ok)
                FirstTestEvent = EventSingleton.Instance.EventCatalog.Collection[0];
            OnPropertyChanged(nameof(FirstTestEvent));
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
