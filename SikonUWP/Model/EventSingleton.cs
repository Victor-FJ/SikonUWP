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
        /// A event that is currently viewed
        /// </summary>
        public Event ViewedEvent { get; set; }

        /// <summary>
        /// A event that is marked for editor pages to use (Is always a shallow copy from the <see cref="EventCatalog"/>)
        /// </summary>
        public Event MarkedEvent
        {
            get => _markedEvent;
            set
            {
                if (value == null)
                {
                    _markedEvent = new Event();
                    MarkedBools = new bool[12];
                    IsNew = true;
                }
                else
                {
                    _markedEvent = value;
                    MarkedBools = Enumerable.Repeat(true, 12).ToArray();
                    IsNew = false;
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
        /// Holds whether the markedevent is new or already exist
        /// </summary>
        public bool IsNew { get; set; }
        /// <summary>
        /// A image that is marked for the creator page
        /// </summary>
        public StorageFile MarkedImage { get; set; }



        private EventSingleton()
        {
            EventCatalog = new EventCatalog(RoomCatalogSingleton.Instance.Rooms, SpeakerCatalogSingleton.Instance.Speakers,
                ImageSingleton.Instance.ImageCatalog.Dictionary.Keys.ToList(), new GenericPersistence<int, Event>("http://localhost:52415/api/Event/"));
            MarkedEvent = null;
        }

        #region TestCatalogs

        //public readonly ObservableCollection<Speaker> Speakers = new ObservableCollection<Speaker>()
        //{
        //    new Speaker("Victor", "2109", "Victor Friis-Jensen", "Jeg er en glad ung gut som godt kan lide rumskibe og wienerbrød og som kun har set sømænd når det regner i roskilde hvor jeg har en uddannelse i melmaling hvor man maler med mel til ære en fyr uden navn og uden hjem i australien ved et vandfald forladt af vand og med en sten i hånden gik han ned afv","Beley"),
        //    new Speaker("Nicolai", "1234", "Nicolai Höyer Christiansen", "Endnu en gut","Beley"),
        //    new Speaker("SebastianEx", "9876", "Sebastian Halkjær Petersen", "Så mange gutter","Beley")
        //};

        //public readonly ObservableCollection<Room> Rooms = new ObservableCollection<Room>()
        //{
        //    new Room("A4.24", "Op af trappen og til venstre, der vil den ligge på højre side", 20),
        //    new Room("A1.01", "Jeg er en glad ung gut som godt kan lide rumskibe og wienerbrød og som kun har set sømænd når det regner i roskilde hvor jeg har en uddannelse i melmaling hvor man maler med mel til ære en fyr uden navn og uden hjem i australien ved et vandfald forladt af vand og med en sten i hånden gik han ned afv", 110),
        //    new Room("B2.11", "Some closet on the right", 5),
        //    new Room("A13.13", "Hen under stien ved siden af det ødelagte spejl", 13)
        //};

        #endregion
    }
}
