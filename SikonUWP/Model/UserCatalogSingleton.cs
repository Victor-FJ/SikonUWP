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
    class UserCatalogSingleton
    {
		private static UserCatalogSingleton _instance = new UserCatalogSingleton();

		public static UserCatalogSingleton Instance
		{
            get
            {
                //if (_instance == null)
                //{
                //    _instance = new UserCatalogSingleton();
                //}

                return _instance;
            }
		}

        public ObservableCollection<User> Users { get; set; }   

        private UserCatalogSingleton()
        {
            Users = new ObservableCollection<User>();
            LoadUsers();
        }

        public async void LoadUsers()
        {
            Users.Clear();
            GenericPersistence<string, User> facade = new GenericPersistence<string, User>("http://localhost:52415/api/BasicUsers");
            List<User> userList = await facade.Get();
            foreach (User user in userList)
            {
                Users.Add(user);
            }
        }

        
	}
}
