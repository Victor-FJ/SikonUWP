using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ModelLibrary.Model;
using SikonUWP.Common;
using SikonUWP.Persistency;

namespace SikonUWP.Model
{
    public class RoomCatalogSingleton
    {
        private static RoomCatalogSingleton _instance = new RoomCatalogSingleton();

        
        public static RoomCatalogSingleton Instance
        {
            get 
            { 
                if (_instance == null)
                {
                    _instance = new RoomCatalogSingleton();//Lazy
                }
                return _instance;
            }
        }

        public ObservableCollection<Room> Rooms { get; set; }

        private RoomCatalogSingleton()
        {
            Rooms = new ObservableCollection<Room>();
        }

        public async void LoadRooms()
        {
            GenericPersistence<string, Room> roomPersistence = 
                new GenericPersistence<string, Room>("http://localhost:52415/api/Room/");
            try
            {
                List<Room> rooms = await roomPersistence.Get();
                foreach (Room room in rooms)
                    Rooms.Add(room);
            }
            catch (HttpRequestException)
            {
                await MessageDialogUtil.MessageDialogAsync("Data Forbindelsen", "Forbindelsen blev ikke oprette");
            }
           

        }
    }
}
