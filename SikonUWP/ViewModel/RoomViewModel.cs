using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel;
using ModelLibrary.Exceptions;
using ModelLibrary.Model;
using SikonUWP.Annotations;
using SikonUWP.Common;
using SikonUWP.Handlers;
using SikonUWP.Model;

namespace SikonUWP.ViewModel
{
    public class RoomViewModel : INotifyPropertyChanged
    {
        public RoomCatalogSingleton RoomCatalog { get; set; }

        public RoomHandler RoomHandler { get; set; }


        /// <summary>
        /// En metode der catcher en fejl, hvis der allerede eksistert et lokale med dette nr.
        /// I starten prøver den at køre metoden CreateRoom for at løbe den igennem exception.
        /// Samt catcher en fejl hvis man ikke har udfyldt alle tekst bokse
        /// </summary>
        public async void Create()
        {
            try
            {
                await RoomHandler.CreateRoom();
            }
            catch (ItIsNotUniqueException inue)
            {
                await MessageDialogUtil.MessageDialogAsync(inue.Message, "Dette rum eksistere allerede");
            }

            catch (EmptyException ex)
            {
                await MessageDialogUtil.MessageDialogAsync(ex.Message, "Du har ikke udfyldt alle felterne");
            }

        }
        
        /// <summary>
        /// Viewmodellens constructor. Her initialiseres og specificeres kommandoernes handlinger.
        /// </summary>
        public RoomViewModel()
        {
            RoomCatalog = RoomCatalogSingleton.Instance;
            RoomHandler = new RoomHandler(this);
            _newRoom = new Room();
            _createRoomCommand = new RelayCommand(Create);
            _deleteRoomCommand = new RelayCommand(RoomHandler.DeleteRoom, SelectedIndexIsNotSet);
            _updateRoomCommand = new RelayCommand(RoomHandler.UpdateRoom, SelectedIndexIsNotSet);
            _clearRoomCommand = new RelayCommand(RoomHandler.ClearRoom, Fade);


            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                Load();
            //SelcectedIndex = -1 gør man ikke direkte er selected til et lokale
            //i det man går ind på lokalets side
            SelectedIndex = -1;
        }

        private async void Load()
        {
            await RoomCatalog.LoadRooms();
        }

        //Denne metode blev brugt til hjælp for at programmet ikke crashede til at starte med, men
        //Blev derefter overflødig.
        //private void StartUp()
        //{
        //    Room r1 = new Room("6B", "Lokalvej 43", 20);
        //    Room r2 = new Room("7B", "Allevej 57", 15);
        //    Room r3 = new Room("16C", "LykkeVænget 18", 100);
        //    Room r4 = new Room("8A", "MangeGård 69", 23);
        //    Room r5 = new Room("9A", "StyreVej 15", 20);

        //    RoomCatalog.Rooms.Add(r1);
        //    RoomCatalog.Rooms.Add(r2);
        //    RoomCatalog.Rooms.Add(r3);
        //    RoomCatalog.Rooms.Add(r4);
        //    RoomCatalog.Rooms.Add(r5);
        //}

        //Commands

        //Create
        private ICommand _createRoomCommand;
        public ICommand CreateRoomCommand => _createRoomCommand;

        //Delete
        private ICommand _deleteRoomCommand;

        public ICommand DeleteRoomCommand => _deleteRoomCommand;

        //Update
        private ICommand _updateRoomCommand;

        public ICommand UpdateRoomCommand => _updateRoomCommand;

        //Clear
        private ICommand _clearRoomCommand;

        public ICommand ClearRoomCommand => _clearRoomCommand;


        //NewRoom
        private Room _newRoom;
        public Room NewRoom
        {
            get { return _newRoom; }
            set
            {
                _newRoom = value;
                if (_newRoom == null)
                {
                    _newRoom = new Room();
                }
                OnPropertyChanged();
                ((RelayCommand)_clearRoomCommand).RaiseCanExecuteChanged();
            }
        }

        //Func
        private int _selectedIndex;

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value; OnPropertyChanged();
                ((RelayCommand)_deleteRoomCommand).RaiseCanExecuteChanged();
                ((RelayCommand)_updateRoomCommand).RaiseCanExecuteChanged();
                
            }
        }

        /// <summary>
        /// Denne metode fader commandoernes knapper i RoomPage, når de ikke er selected til et lokale. 
        /// </summary>
        /// <returns>Faded knap</returns>
        public bool SelectedIndexIsNotSet()
        {
            return SelectedIndex != -1;

        }


        /// <summary>
        /// Metode der gør man kan selecte et lokale
        /// </summary>
        private Room _selectedRoom;

        public Room SelectedRoom
        {
            get { return _selectedRoom; }
            set
            {
                _selectedRoom = value;
                if (_selectedRoom != null)
                    NewRoom = new Room (_selectedRoom.RoomNo, _selectedRoom.LocationDescription, _selectedRoom.MaxNoPeople);
                
                OnPropertyChanged(); }
        }

        /// <summary>
        /// Fader clear commandoen når der ikke er noget i tekstbokserne
        /// </summary>
        /// <returns>Fading</returns>
        public bool Fade()
        {
            return NewRoom.RoomNo != null && NewRoom.LocationDescription != null && NewRoom.MaxNoPeople != 0;
        }



        //Property Change

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
