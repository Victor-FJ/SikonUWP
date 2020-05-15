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
    class ParticipantCatalogSingleton
    {
        private ParticipantCatalogSingleton _instance = null;

		public ParticipantCatalogSingleton Instance
		{
            get
            {
                if (_instance == null)
                {
                    _instance = new ParticipantCatalogSingleton();
                }
                return _instance;
            }
			set { _instance = value; }
		}

		public  ObservableCollection<Participant> Participants { get; set; }

        private ParticipantCatalogSingleton()
        {
			Participants = new ObservableCollection<Participant>();
            LoadParticipants();
        }

        public async void LoadParticipants()
        {
            Participants.Clear();
            GenericPersistence<string, Participant> facade = new GenericPersistence<string, Participant>("http://localhost:52415/api/Participants");
            List<Participant> participantList = await facade.Get();
            foreach (Participant user in participantList)
            {
                Participants.Add(user);
            }
        }
	}
}
