using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLibrary.Model;
using SikonUWP.Persistency;
using SikonUWP.ViewModel;

namespace SikonUWP.Handlers
{
    class ParticipantHandler
    {
        //public ParticipantCreatorViewModel ParticipantCreatorViewModel { get; set; }

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
        }

        public async void UpdateParticipant(Participant participant)
        {
            await participantFacade.Put(participant.UserName, participant);
        }

        public async void DeleteParticipant(Participant participant)
        {
            await participantFacade.Delete(participant.UserName);
        }

    }
}
