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
    public class AdminCatalogSingleton
    {
		private static AdminCatalogSingleton _instance = null;

		public static AdminCatalogSingleton Instance
		{
            get
            {
                if (_instance == null)
                {
                    _instance = new AdminCatalogSingleton();
                }
                return _instance;
            }
			set { _instance = value; }
		}

        public ObservableCollection<Admin> Admins { get; set; }

        private AdminCatalogSingleton()
        {
            Admins=new ObservableCollection<Admin>();
        }

        public async Task LoadAdmins()
        {
            Admins.Clear();
            GenericPersistence<string, Admin> facade = new GenericPersistence<string, Admin>("http://localhost:52415/api/Admins");
            List<Admin> adminList = await facade.Get();
            foreach (Admin user in adminList)
            {
                Admins.Add(user);
            }
        }
    }
}
