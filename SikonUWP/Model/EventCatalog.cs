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


        private readonly GenericPersistence<int, Event> _eventPersistence;

        public ObservableCollection<Room> Rooms { get; set; }

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
        public bool Add(Event @event, bool getId)
        {
            List<Event> collection = _collection.ToList();
            if (collection.Find((x) => x.Id == @event.Id) != null)
                if (getId)
                    @event.Id = GetUniqueId();
                else
                    throw new BaseException(""); //TODO: Throw uniqe exception instead

            CheckDate(@event);
            return false;
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
            if (selectedEvent.Room == null || !_collection.Contains(selectedEvent))
                throw new EmptyException("Lokalet eksistere ikke ");
        }


        public void CheckDate(Event selectedEvent)
        {
            List<Event> possConSpeakerEvents = (from @event in _collection
                where selectedEvent.Speaker == @event.Speaker
                select @event).ToList();

            foreach (Event possConEvent in possConSpeakerEvents)
                if (selectedEvent.StartDate < possConEvent.EndDate && selectedEvent.EndDate > possConEvent.StartDate)
                    throw new OutsideRangeException("Værten er optaget af en anden begivenhed på det tidspunkt");

            List<Event> possConRoomEvents = (from @event in _collection
                where selectedEvent.Room == @event.Room
                select @event).ToList();

            foreach (Event possConEvent in possConSpeakerEvents)
                if (selectedEvent.StartDate < possConEvent.EndDate && selectedEvent.EndDate > possConEvent.StartDate)
                    throw new OutsideRangeException("Lokalet er brugt af en anden begivenhed på det tidspunkt");
        }
    }
}
