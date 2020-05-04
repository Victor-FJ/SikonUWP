using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using ModelLibrary.Model;
using SikonUWP.Persistency;
using Enumerable = System.Linq.Enumerable;

namespace SikonUWP.Model
{
    public class EventSingleton
    {
        /// <summary>
        /// Instance of singleton class
        /// </summary>
        public static readonly EventSingleton Instance = new EventSingleton();

        
        /// <summary>
        /// The list of all events from the database
        /// </summary>
        public EventCatalog EventCatalog { get; set; }


        /// <summary>
        /// A event that is marked for other pages to use (Is always a shallow copy from the <see cref="EventCatalog"/>)
        /// </summary>
        public Event MarkedEvent
        {
            get => _markedEvent;
            set
            {
                if (value == null)
                {
                    _markedEvent = new Event();
                    MarkedBools = new bool[10];
                }
                else
                {
                    _markedEvent = value;
                    MarkedBools = Enumerable.Repeat(true, 10).ToArray();
                }
                MarkedImage = null;
            }
        }
        private Event _markedEvent;
        /// <summary>
        /// Holds witch values is completed
        /// </summary>
        public bool[] MarkedBools { get; set; }
        /// <summary>
        /// A image that is marked for the creator page
        /// </summary>
        public StorageFile MarkedImage { get; set; }



        private EventSingleton()
        {
            EventCatalog = new EventCatalog(new GenericPersistence<int, Event>("http://localhost:52415/api/Event/"));
            Load();
        }

        private async void Load()
        {
            await EventCatalog.Load();
        }
    }
}
