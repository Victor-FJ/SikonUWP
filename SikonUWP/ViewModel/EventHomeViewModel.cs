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

        private int _number;
        public int Number
        {
            get => _number;
            set
            {
                _number = value;
                if (EventSingleton.Instance.EventCatalog.Collection.Count > value)
                    FirstTestEvent = EventSingleton.Instance.EventCatalog.Collection[value];
                OnPropertyChanged(nameof(FirstTestEvent));
            }
        }

        public EventHomeViewModel()
        {
            Load();
        }

        private async void Load()
        {
            bool ok = await EventSingleton.Instance.EventCatalog.Load();
            Number = 0;
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
