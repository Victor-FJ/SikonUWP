using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLibrary.Model;

namespace SikonUWP.Model
{
    class AdminCatalogSingleton
    {
		private static AdminCatalogSingleton _instance;

		public static AdminCatalogSingleton Instance
		{
			get { return _instance; }
			set { _instance = value; }
		}

		public UserCatalogSingleton UserCatalogSingleton { get; set; }
        public ObservableCollection<Admin> Admins { get; set; }

        private AdminCatalogSingleton()
        {
            Admins=new ObservableCollection<Admin>();
            UserCatalogSingleton = UserCatalogSingleton.Instance;
            LoadAdmins();
        }


        public void LoadAdmins()
        {
            foreach (User user in UserCatalogSingleton.Users)
            {
                if (user is Admin admin)
                {
                    Admins.Add(admin);
                }
            }
        }
    }
}
