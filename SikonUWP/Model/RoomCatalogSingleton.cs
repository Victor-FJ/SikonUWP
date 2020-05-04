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

            Room r1 = new Room("6B", "Lokalvej 43", 20);
            Room r2 = new Room("7B", "Allevej 57", 15);
            Room r3 = new Room("16C", "LykkeVænget 18", 100);
            Room r4 = new Room("8A", "MangeGård 69", 23);
            Room r5 = new Room("9A", "StyreVej 15", 20);

            Rooms.Add(r1);
            Rooms.Add(r2);
            Rooms.Add(r3);
            Rooms.Add(r4);
            Rooms.Add(r5);

        }

        public async void LoadRooms()
        {
            GenericPersistence<string, Room> roomPersistence = 
                new GenericPersistence<string, Room>("http://localhost:52415/api/Room/");
            List<Room> rooms = await roomPersistence.Get();
            foreach (Room room in rooms)
                Rooms.Add(room);

        }
    }
}
