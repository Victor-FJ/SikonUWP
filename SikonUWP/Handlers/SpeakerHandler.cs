using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLibrary.Model;
using SikonUWP.Model;
using SikonUWP.Persistency;

namespace SikonUWP.Handlers
{
    class SpeakerHandler
    {
        private readonly SpeakerCatalogSingleton _speakerCat = SpeakerCatalogSingleton.Instance;

        public GenericPersistence<string, Speaker> SpeakerFacade;

        private const string SpeakerURI = "http://localhost:52415/api/Speakers";

        public SpeakerHandler()
        {
            SpeakerFacade = new GenericPersistence<string, Speaker>(SpeakerURI);
        }

        public async void CreateSpeaker(Speaker speaker)
        {
            await SpeakerFacade.Post(speaker);
            //Tilføjer til cataloget så reload er unødvendig
            SpeakerCatalogSingleton.Instance.Speakers.Add(speaker);
        }

        public async void UpdateSpeaker(Speaker speaker)
        {
            await SpeakerFacade.Put(speaker.UserName, speaker);
            //Opdatere cataloget så reload er unødvendig
            Speaker oldSpeaker = _speakerCat.Speakers.First(x => x.UserName == speaker.UserName);
            int index = _speakerCat.Speakers.IndexOf(oldSpeaker);
            _speakerCat.Speakers.Insert(index, speaker);
        }

        public async void DeleteSpeaker(Speaker speaker)
        {
            await SpeakerFacade.Delete(speaker.UserName);
            //Sletter til cataloget så reload er unødvendig
            Speaker oldSpeaker = _speakerCat.Speakers.First(x => x.UserName == speaker.UserName);
            _speakerCat.Speakers.Remove(oldSpeaker);
        }
    }
}
