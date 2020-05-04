using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLibrary.Model;
using SikonUWP.Persistency;

namespace SikonUWP.Handlers
{
    class SpeakerHandler
    {
        public GenericPersistence<string, Speaker> SpeakerFacade;

        private const string SpeakerURI = "http://localhost:52415/api/Speakers";

        public SpeakerHandler()
        {
            SpeakerFacade = new GenericPersistence<string, Speaker>(SpeakerURI);
        }

        public async void CreateSpeaker(Speaker speaker)
        {
            await SpeakerFacade.Post(speaker);
        }

        public async void UpdateSpeaker(Speaker speaker)
        {
            await SpeakerFacade.Put(speaker.UserName, speaker);
        }

        public async void DeleteSpeaker(Speaker speaker)
        {
            await SpeakerFacade.Delete(speaker.UserName);
        }
    }
}
