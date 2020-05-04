using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Text;
using ModelLibrary.Model;
using SikonUWP.Common;
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

        public async void CreateRoom()
        {
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
    }
}
