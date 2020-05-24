using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.FileProperties;
using ModelLibrary.Exceptions;
using ModelLibrary.Model;
using SikonUWP.Persistency;

namespace SikonUWP.Model
{
    public class RegistrationSingleton
    {
        /// <summary>
        /// Instance of singleton class
        /// </summary>
        public static readonly RegistrationSingleton Instance = new RegistrationSingleton();

        public EventSingleton EventSing { get; set; }
        public ParticipantCatalogSingleton ParticiSing { get; set; }

        public List<Registration> Registrations { get; set; }
        public Dictionary<Event, ObservableCollection<Participant>> RegistrationDictionary { get; set; }

        private const string _uri = "http://localhost:52415/api/Registration/";
        private readonly GenericPersistence<int, Registration> _registrationPersistence = new GenericPersistence<int, Registration>(_uri);

        private RegistrationSingleton()
        {
            RegistrationDictionary = new Dictionary<Event, ObservableCollection<Participant>>();
            EventSing = EventSingleton.Instance;
            ParticiSing = ParticipantCatalogSingleton.Instance;
        }

        public async Task Load()
        {
            Registrations = await _registrationPersistence.Get();

            RegistrationDictionary = (from @event in EventSing.EventCatalog.Collection
                select new
                {
                    @event,
                    participants = new ObservableCollection<Participant>(from registration in Registrations 
                        join participant in ParticiSing.Participants on registration.UserName equals participant.UserName 
                        where registration.EventId == @event.Id select participant)
                }).ToDictionary(x => x.@event, x => x.participants);
        }

        public void AddEvent(Event @event)
        {
            RegistrationDictionary.Add(@event, new ObservableCollection<Participant>());
        }

        public void UpdateEvent(Event oldEvent, Event @event)
        {
            ObservableCollection<Participant> oldEventRegi = RegistrationDictionary[oldEvent];
            RegistrationDictionary.Remove(oldEvent);
            RegistrationDictionary.Add(@event, oldEventRegi);
        }

        public async Task DeleteEvent(Event oldEvent)
        {
            await ClearRegistration(oldEvent);
            RegistrationDictionary.Remove(oldEvent);
        }

        public async Task AddRegistration(Event @event, Participant participant)
        {
            Registration registration = new Registration(GetUniqueId(), participant.UserName, @event.Id);
            bool ok = await _registrationPersistence.Post(registration);
            if (ok)
            {
                Registrations.Add(registration);
                RegistrationDictionary[@event].Add(participant);
            }
            else
                throw new BaseException("Add regi fail");
        }

        public async Task RemoveRegistration(Event @event, Participant participant)
        {
            Registration registration =
                Registrations.Find(x => x.EventId == @event.Id && x.UserName == participant.UserName);
            bool ok = await _registrationPersistence.Delete(registration.Id);
            if (ok)
            {
                Registrations.Remove(registration);
                RegistrationDictionary[@event].Remove(participant);
            }
            else
                throw new BaseException("Remove regi fail");
        }

        public async Task ClearRegistration(Event @event)
        {
            bool ok = await CustomPersistence.Delete(@event.Id, _uri + "ClearEvent/");
            if (ok)
            {
                Registrations.RemoveAll(x => x.EventId == @event.Id);
                RegistrationDictionary[@event].Clear();
            }
            else
                throw new BaseException("Clear regi fail");
        }

        public int GetUniqueId()
        {
            List<int> orderedList = (from registration in Registrations orderby registration.Id select registration.Id).ToList();
            if (orderedList.Count == 0)
                return 0;
            int uniqueId = orderedList[0] + 1;
            for (int i = 1; i < orderedList.Count(); i++)
                if (orderedList[i] != uniqueId)
                    return uniqueId;
                else
                    uniqueId++;
            return uniqueId;
        }
    }
}
