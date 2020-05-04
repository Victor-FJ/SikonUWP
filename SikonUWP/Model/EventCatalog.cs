using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLibrary.Model;

namespace SikonUWP.Model
{
    public class EventCatalog
    {
        public ReadOnlyObservableCollection<Event> Collection { get; set; }

        private ObservableCollection<Event> _collection;


        public EventCatalog()
        {
            _collection = new ObservableCollection<Event>();
            Collection = new ReadOnlyObservableCollection<Event>(_collection);
        }
    }
}
