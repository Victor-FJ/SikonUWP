using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLibrary.Model;
using SikonUWP.Model;
using SikonUWP.Persistency;
using SikonUWP.ViewModel;

namespace SikonUWP.Handlers
{
    class ParticipantHandler
    {
        //public ParticipantCreatorViewModel ParticipantCreatorViewModel { get; set; }

        private readonly ParticipantCatalogSingleton _participantCat = ParticipantCatalogSingleton.Instance;

        private GenericPersistence<string, Participant> participantFacade;

        private const string ParticipantURI = "http://localhost:52415/api/Participants";

        public ParticipantHandler()
        {
            //ParticipantCreatorViewModel = new ParticipantCreatorViewModel();
            participantFacade = new GenericPersistence<string, Participant>(ParticipantURI);
        }

        public async void CreateParticipant(Participant participant)
        {
            await participantFacade.Post(participant);
            //Tilføjer til cataloget så reload er unødvendig
            ParticipantCatalogSingleton.Instance.Participants.Add(participant);
        }

        public async void UpdateParticipant(Participant participant)
        {
            await participantFacade.Put(participant.UserName, participant);
            //Opdatere cataloget så reload er unødvendig
            Participant oldParticipant = _participantCat.Participants.First(x => x.UserName == participant.UserName);
            int index = _participantCat.Participants.IndexOf(oldParticipant);
            _participantCat.Participants.Insert(index, participant);
        }

        public async void DeleteParticipant(Participant participant)
        {
            await participantFacade.Delete(participant.UserName);
            //Sletter til cataloget så reload er unødvendig
            Participant oldParticipant = _participantCat.Participants.First(x => x.UserName == participant.UserName);
            _participantCat.Participants.Remove(oldParticipant);
        }

    }
}
