using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLibrary.Model;
using SikonUWP.Persistency;

namespace SikonUWP.Model
{
    public class EventCatalogSingleton
    {
        public static readonly EventCatalogSingleton Instance = new EventCatalogSingleton();


        public GenericPersistence<int, Event> EventPersistence { get; set; }

        public List<Event> EventCatalog { get; set; }


        private EventCatalogSingleton()
        {
            EventPersistence = new GenericPersistence<int, Event>("http://localhost:52415/api/Event/");
        }


    }
}
