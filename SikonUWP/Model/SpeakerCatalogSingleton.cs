using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLibrary.Model;
using SikonUWP.Persistency;

namespace SikonUWP.Model
{
    class SpeakerCatalogSingleton
    {
		private static SpeakerCatalogSingleton _instance = null;

		public static SpeakerCatalogSingleton Instance
		{
            get
            {
                if (_instance == null)
                {
                    _instance = new SpeakerCatalogSingleton();
                }
                return _instance;
            }
			set { _instance = value; }
		}


		public ObservableCollection<Speaker> Speakers { get; set; }

        private SpeakerCatalogSingleton()
        {
            Speakers = new ObservableCollection<Speaker>();
            LoadSpeakers();
        }

        public async void LoadSpeakers()
        {
            Speakers.Clear();
            GenericPersistence<string, Speaker> facade = new GenericPersistence<string, Speaker>("http://localhost:52415/api/Speakers");
            List<Speaker> speakerList = await facade.Get();
            foreach (Speaker user in speakerList)
            {
                Speakers.Add(user);
            }
        }

	}
}
