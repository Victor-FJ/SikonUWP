using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLibrary.Model;

namespace SikonUWP.Model
{
    class SpeakerCatalogSingleton
    {
		private static SpeakerCatalogSingleton _instance;

		public static SpeakerCatalogSingleton Instance
		{
			get { return _instance; }
			set { _instance = value; }
		}




        public UserCatalogSingleton UserCatalogSingleton { get; set; }
		public ObservableCollection<Speaker> Speakers { get; set; }

        private SpeakerCatalogSingleton()
        {
            Speakers = new ObservableCollection<Speaker>();
            UserCatalogSingleton = UserCatalogSingleton.Instance;
            LoadSpeakers();
        }

        public void LoadSpeakers()
        {
            foreach (User user in UserCatalogSingleton.Users)
            {
                if (user is Speaker speaker)
                {
                    Speakers.Add(speaker);
                }
            }
        }

	}
}
