using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using ModelLibrary.Exceptions;
using ModelLibrary.Model;
using SikonUWP.Persistency;
using SikonUWP.ViewModel;

namespace SikonUWP.Model
{
    public class EventCatalog
    {
        public ReadOnlyObservableCollection<Event> Collection { get; private set; }

        private ObservableCollection<Event> _collection;

        private MainViewModel _main = MainViewModel.Instance;

        private readonly GenericPersistence<int, Event> _eventPersistence;

        public EventCatalog(GenericPersistence<int, Event> eventPersistence)
        {
            _collection = new ObservableCollection<Event>();
            Collection = new ReadOnlyObservableCollection<Event>(_collection);
            _eventPersistence = eventPersistence;
        }

        public async Task<bool> Load()
        {
            try
            {
                _collection = new ObservableCollection<Event>(await _eventPersistence.Get());
                Collection = new ReadOnlyObservableCollection<Event>(_collection);
                foreach (Event @event in _collection)
                {
                    
                    @event.Room = RoomCatalogSingleton.Instance.Rooms.Single((x) => x.RoomNo == @event.Room.RoomNo);
                    @event.Speaker = SpeakerCatalogSingleton.Instance.Speakers.Single((x) => x.UserName == @event.Speaker.UserName);
                }
                return true;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        /// <summary>
        /// Add an event to the catalog
        /// </summary>
        /// <param name="event">The event to add</param>
        /// <param name="getId">If true an new id will be made if the events id is not unique if false and exception will be thrown if its not</param>
        /// <returns>Whether the event was succesfully added</returns>
        public async Task<bool> Add(Event @event, bool getId)
        {
            if (_collection.SingleOrDefault((x) => x.Id == @event.Id) != null)
                if (getId)
                    @event.Id = GetUniqueId();
                else
                    throw new ItIsNotUniqueException("Begivenhedens id er ikke unikt");
            CheckSpeaker(@event);
            CheckRoom(@event);
            CheckDate(@event);
            CheckImage(@event, false);

            _collection.Add(@event);
            bool ok = await _eventPersistence.Post(@event);
            if (!ok)
            {
                _collection.Remove(@event);
                throw new BaseException("Thing no. 1. you thought would never happen. But of course it did");
            }
            return true;
        }

        public async Task<bool> Update(int id, Event @event)
        {
            Event oldEvent = _collection.Single((x) => x.Id == id);
            int index = _collection.IndexOf(oldEvent);
            _collection.Remove(oldEvent);
            CheckSpeaker(@event);
            CheckRoom(@event);
            CheckDate(@event);
            CheckImage(@event, false);

            _collection.Insert(index, @event);
            bool ok = await _eventPersistence.Put(id, @event);
            if (!ok)
            {
                _collection.Remove(@event);
                _collection.Insert(index, oldEvent);
                throw new BaseException("Thing no. 2. you thought would never happen. But of course it did");
            }

            return true;
        }

        public async Task<bool> Remove(int id)
        {
            Event @event = _collection.Single((x) => x.Id == id);

            _collection.Remove(@event);
            bool ok = await _eventPersistence.Delete(id);
            if (!ok)
            {
                _collection.Add(@event);
                throw new BaseException("Thing no. 3. you thought would never happen. But of course it did");
            }

            return true;
        }

        public int GetUniqueId()
        {
            List<int> orderedList = (from @event in _collection orderby @event.Id select @event.Id).ToList();
            int uniqueId = orderedList[0] + 1;
            for (int i = 1; i < orderedList.Count(); i++)
                if (orderedList[i] != uniqueId)
                    return uniqueId;
                else
                    uniqueId++;
            return uniqueId;
        }

        public void CheckSpeaker(Event selectedEvent)
        {
            if (selectedEvent.Speaker == null || !SpeakerCatalogSingleton.Instance.Speakers.Contains(selectedEvent.Speaker))
                throw new EmptyException("Værten eksistere ikke i cataloget");
        }

        public void CheckRoom(Event selectedEvent)
        {
            if (selectedEvent.Room == null || !RoomCatalogSingleton.Instance.Rooms.Contains(selectedEvent.Room))
                throw new EmptyException("Lokalet eksistere ikke i cataloget");
        }


        public void CheckDate(Event selectedEvent)
        {
            int speakerConflicts = (from @event in _collection
                                    where selectedEvent.Speaker == @event.Speaker && selectedEvent.StartDate < @event.EndDate &&
                                          selectedEvent.EndDate > @event.StartDate && selectedEvent.Id != @event.Id
                                    select @event).Count();

            if (speakerConflicts != 0)
                throw new OutsideRangeException("Værten er optaget på dette tidspunkt");

            int roomConflicts = (from @event in _collection
                                 where selectedEvent.Room == @event.Room && selectedEvent.StartDate < @event.EndDate &&
                                       selectedEvent.EndDate > @event.StartDate && selectedEvent.Id != @event.Id
                                 select @event).Count();

            if (roomConflicts != 0)
                throw new OutsideRangeException("Lokalet bliver brugt på dette tidspunktet");
        }

        public void CheckImage(Event selectedEvent, bool beUnique)
        {
            bool doesContain = ImageSingleton.Instance.ImageCatalog.Dictionary.ContainsKey(selectedEvent.ImageName);
            if (beUnique && doesContain)
                throw new ItIsNotUniqueException("Der er allerede et billed med det navn");
            if (!beUnique && !doesContain)
                throw new ItIsUniqueException("Der er intet billed med det navn");
        }
    }
}
