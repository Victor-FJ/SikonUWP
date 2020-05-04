using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLibrary.Model;
using SikonUWP.Persistency;

namespace SikonUWP.Handlers
{
    class AdminHandler
    {
        public GenericPersistence<string, Admin> adminHandler;

        private const string adminURI = "http://localhost:52415/api/Admins";

        public AdminHandler()
        {
            adminHandler = new GenericPersistence<string, Admin>(adminURI);
        }


        public async void CreateAdmin(Admin admin)
        {
            await adminHandler.Post(admin);
        }

        public async void UpdateAdmin(Admin admin)
        {
            await adminHandler.Put(admin.UserName, admin);
        }

        public async void DeleteAdmin(Admin admin)
        {
            await adminHandler.Delete(admin.UserName);
        }
    }
}
