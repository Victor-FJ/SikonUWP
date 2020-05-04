using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Text;
using ModelLibrary.Exceptions;
using ModelLibrary.Model;
using SikonUWP.Common;
using SikonUWP.Model;
using SikonUWP.Persistency;
using SikonUWP.ViewModel;

namespace SikonUWP.Handlers
{
    public class RoomHandler
    {
        public RoomViewModel RoomViewModel { get; set; }

        public RoomHandler(RoomViewModel roomViewModel)
        {
            RoomViewModel = roomViewModel;
        }

        private GenericPersistence<string, Room> _genericPersistence = new GenericPersistence<string, Room>("http://localhost:52415/api/Room/");

        public async Task CreateRoom()
        {
            Validate();
            bool ok = await _genericPersistence.Post(RoomViewModel.NewRoom);

            if (!ok)
            {
                await MessageDialogUtil.MessageDialogAsync("Der skete en fejl i Room", "Lokalet blev ikke oprettet");
            }
            else
            {
                await MessageDialogUtil.MessageDialogAsync("Alt gik godt", $"Lokalet blev oprettet");
                RoomViewModel.RoomCatalog.Rooms.Clear();
                RoomViewModel.NewRoom = new Room();
                RoomViewModel.RoomCatalog.LoadRooms();
                
            }
        }

        public async void DeleteRoom()
        {
            string roomNo = RoomViewModel.SelectedRoom.RoomNo;
            bool ok = await _genericPersistence.Delete(roomNo);

            if (!ok)
            {
                await MessageDialogUtil.MessageDialogAsync("Der skete en fejl i Room", "Room blev ikke slettet");
            }
            else
            {
                await MessageDialogUtil.MessageDialogAsync("Alt gik godt", $"Lokalet {roomNo} blev sletet");
                RoomViewModel.RoomCatalog.Rooms.Clear();
                RoomViewModel.NewRoom = new Room();
                RoomViewModel.RoomCatalog.LoadRooms();

            }
        }

        public async void UpdateRoom()
        {
            string roomNo = RoomViewModel.SelectedRoom.RoomNo;
            bool ok = await _genericPersistence.Put(roomNo, RoomViewModel.NewRoom);

            if (!ok)
            {
                await MessageDialogUtil.MessageDialogAsync("Der skete en fejl i Room", "Room blev ikke opdateret");
            }
            else
            {
                await MessageDialogUtil.MessageDialogAsync("Alt gik godt", $"Lokalet {roomNo} blev opdateret");
                RoomViewModel.RoomCatalog.Rooms.Clear();
                RoomViewModel.NewRoom = new Room();
                RoomViewModel.RoomCatalog.LoadRooms();

            }
        }

        public void ClearRoom()
        {
            RoomViewModel.NewRoom = new Room();
        }


            private void Validate()
            {
                List<Room> collection = RoomCatalogSingleton.Instance.Rooms.ToList();
                if (collection.Find((x) => x.RoomNo == RoomViewModel.NewRoom.RoomNo) != null)
                {
                    throw new ItIsNotUniqueException("Dette dørnummer er allerede i Listen");
                }

            }
        


       
    }
}
