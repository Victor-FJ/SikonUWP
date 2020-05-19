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

        /// <summary>
        /// Denne metode bliver brugt til at oprette et lokale
        /// </summary>
        /// <returns>et nyt lokale</returns>
        public async Task CreateRoom()
        {
            Validate();
            if (DoesExist())
                throw new ItIsNotUniqueException("Something");

            bool ok = await _genericPersistence.Post(RoomViewModel.NewRoom);

            if (!ok)
            {
                await MessageDialogUtil.MessageDialogAsync("Der skete en fejl i Room", "Lokalet blev ikke oprettet");
            }
            else
            {
                await MessageDialogUtil.MessageDialogAsync("Alt gik godt", $"Lokalet blev oprettet");
                RoomViewModel.RoomCatalog.Rooms.Add(RoomViewModel.NewRoom);
                RoomViewModel.NewRoom = new Room();
                
                
            }
        }

        /// <summary>
        /// Denne metode bliver brugt til at slette et lokale
        /// </summary>
        public async void DeleteRoom()
        {
            bool sure = await MessageDialogUtil.InputDialogAsync("Hvad?", "er du sikker på du vil slette");
            if (!sure)
            {
                return;
            }

            string roomNo = RoomViewModel.SelectedRoom.RoomNo;
            bool ok = await _genericPersistence.Delete(roomNo);

            if (!ok)
            {
                await MessageDialogUtil.MessageDialogAsync("Der skete en fejl i Room", "Room blev ikke slettet");
            }
            else
            {
                await MessageDialogUtil.MessageDialogAsync("Alt gik godt", $"Lokalet {roomNo} blev slettet");
                RoomViewModel.RoomCatalog.Rooms.Remove(RoomViewModel.SelectedRoom);
                RoomViewModel.NewRoom = new Room();
                

            }
        }

        /// <summary>
        /// Denne medtode bliver bruger man til at opdaterer et lokale
        /// </summary>
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
               int index = RoomViewModel.RoomCatalog.Rooms.IndexOf(RoomViewModel.SelectedRoom);
               RoomViewModel.RoomCatalog.Rooms[index] = RoomViewModel.NewRoom;
                RoomViewModel.NewRoom = new Room();

            }
        }

        /// <summary>
        /// Denne metode bliver brugt til at rydde felterne i skriveboksne.
        /// </summary>
        public void ClearRoom()
        {
            RoomViewModel.NewRoom = new Room();
            RoomViewModel.SelectedIndex = - 1;
        }

        /// <summary>
        /// Denne metode bliver kaldt, hvis man prøver at oprette et lokale med samme dørnummer
        /// </summary>
        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(RoomViewModel.NewRoom.RoomNo))
            {
                throw new EmptyException("Tomt felt i RoomNo");
            }

            if (string.IsNullOrWhiteSpace(RoomViewModel.NewRoom.LocationDescription))
            {
                throw new EmptyException("Tomt felt i LocationsDes");
            }
        }

        private bool DoesExist()
        {
            List<Room> collection = RoomCatalogSingleton.Instance.Rooms.ToList();
            return collection.Find((x) => x.RoomNo == RoomViewModel.NewRoom.RoomNo) != null;
        }


    }
}
