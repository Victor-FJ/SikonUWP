using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using ModelLibrary.Exceptions;
using ModelLibrary.Model;
using SikonUWP.Persistency;

namespace SikonUWP.Model
{
    public class EventCatalog
    {
        public ReadOnlyObservableCollection<Event> Collection { get; private set; }

        private ObservableCollection<Event> _collection;


        private readonly ObservableCollection<Room> _rooms;
        private readonly ObservableCollection<Speaker> _speakers;
        private readonly List<string> _imageNames;

        private readonly GenericPersistence<int, Event> _eventPersistence;

        public EventCatalog(ObservableCollection<Room> rooms, ObservableCollection<Speaker> speakers, List<string> imageNames, GenericPersistence<int, Event> eventPersistence)
        {
            _collection = new ObservableCollection<Event>();
            Collection = new ReadOnlyObservableCollection<Event>(_collection);
            _rooms = rooms;
            _speakers = speakers;
            _imageNames = imageNames;
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
                    @event.Room = _rooms.Single((x) => x.RoomNo == @event.Room.RoomNo);
                    @event.Speaker = _speakers.Single((x) => x.UserName == @event.Speaker.UserName);
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
            if (_collection.Single((x) => x.Id == @event.Id) != null)
                if (getId)
                    @event.Id = GetUniqueId();
                else
                    throw new ItIsNotUniqueException("Begivenhedens id er ikke unikt");
            CheckSpeaker(@event);
            CheckRoom(@event);
            CheckDate(@event);
            CheckImage(@event, true);

            bool ok = await _eventPersistence.Post(@event);
            if (ok)
                _collection.Add(@event);
            return ok;
        }

        public async Task<bool> Update(int id, Event @event)
        {
            Event oldEvent = _collection.Single((x) => x.Id == id);
            CheckSpeaker(@event);
            CheckRoom(@event);
            CheckDate(@event);
            CheckImage(@event, true);

            bool ok = await _eventPersistence.Put(id, @event);
            if (ok)
            {
                _collection.Remove(oldEvent);
                _collection.Add(@event);
            }
            return ok;
        }

        public async Task<bool> Remove(int id)
        {
            Event @event = _collection.Single((x) => x.Id == id);

            bool ok = await _eventPersistence.Delete(id);
            if (ok)
                _collection.Remove(@event);
            return ok;
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
            if (selectedEvent.Room == null || !_speakers.Contains(selectedEvent.Speaker))
                throw new EmptyException("Værten eksistere ikke i cataloget");
        }

        public void CheckRoom(Event selectedEvent)
        {
            if (selectedEvent.Room == null || !_rooms.Contains(selectedEvent.Room))
                throw new EmptyException("Lokalet eksistere ikke i cataloget");
        }


        public void CheckDate(Event selectedEvent)
        {
            int speakerConflicts = (from @event in _collection
                where selectedEvent.Speaker == @event.Speaker && selectedEvent.StartDate < @event.EndDate && selectedEvent.EndDate > @event.StartDate
                select @event).Count();

            if (speakerConflicts != 0)
                throw new OutsideRangeException("Værten er optaget på dette tidspunkt");

            int roomConflicts = (from @event in _collection
                where selectedEvent.Room == @event.Room && selectedEvent.StartDate < @event.EndDate &&
                      selectedEvent.EndDate > @event.StartDate
                select @event).Count();
            
            if (roomConflicts != 0) 
                throw new OutsideRangeException("Lokalet bliver brugt på dette tidspunktet");
        }

        public void CheckImage(Event selectedEvent, bool beUnique)
        {
            bool doesContain = _imageNames.Contains(selectedEvent.ImageName);
            if (beUnique && doesContain)
                throw new ItIsNotUniqueException("Der er allerede et billed med det navn");
            if (!beUnique && !doesContain)
                throw new ItIsUniqueException("Der er intet billed med det navn");
        }
    }
}
