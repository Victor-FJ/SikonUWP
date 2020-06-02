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
    class AdminHandler
    {
        private readonly AdminCatalogSingleton _adminCat = AdminCatalogSingleton.Instance;

        public GenericPersistence<string, Admin> adminHandler;

        private const string adminURI = "http://localhost:52415/api/Admins";

        public AdminHandler()
        {
            adminHandler = new GenericPersistence<string, Admin>(adminURI);
        }


        public async void CreateAdmin(Admin admin)
        {
            await adminHandler.Post(admin);
            //Tilføjer til cataloget så reload er unødvendig
            AdminCatalogSingleton.Instance.Admins.Add(admin);
        }

        public async void UpdateAdmin(Admin admin)
        {
            await adminHandler.Put(admin.UserName, admin);
            //Opdatere cataloget så reload er unødvendig
            Admin oldAdmin = _adminCat.Admins.First(x => x.UserName == admin.UserName);
            int index = _adminCat.Admins.IndexOf(oldAdmin);
            _adminCat.Admins.Insert(index, admin);
        }

        public async void DeleteAdmin(Admin admin)
        {
            await adminHandler.Delete(admin.UserName);
            //Sletter til cataloget så reload er unødvendig
            Admin oldAdmin = _adminCat.Admins.First(x => x.UserName == admin.UserName);
            _adminCat.Admins.Remove(oldAdmin);
        }
    }
}
