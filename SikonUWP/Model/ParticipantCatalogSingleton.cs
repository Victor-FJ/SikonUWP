using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLibrary.Model;

namespace SikonUWP.Model
{
    class ParticipantCatalogSingleton
    {
		private ParticipantCatalogSingleton _instance;

		public ParticipantCatalogSingleton Instance
		{
			get { return _instance; }
			set { _instance = value; }
		}

        private UserCatalogSingleton userCatalogSingleton { get; set; }
		public  ObservableCollection<Participant> Participants { get; set; }

        private ParticipantCatalogSingleton()
        {
			Participants = new ObservableCollection<Participant>();
            userCatalogSingleton = UserCatalogSingleton.Instance;
            LoadParticipants();
        }

        public void LoadParticipants()
        {
            foreach (User user in userCatalogSingleton.Users)
            {
                if (user is Participant participant)
                {
                    Participants.Add(participant);
                }
            }
        }
	}
}
