using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ModelLibrary.Model;
using SikonUWP.Annotations;
using SikonUWP.Common;
using SikonUWP.Handlers;
using SikonUWP.Model;

namespace SikonUWP.ViewModel
{
    public class RoomViewModel: INotifyPropertyChanged
    {
        public RoomCatalogSingleton RoomCatalog { get; set; }

        public RoomHandler RoomHandler { get; set; }

        
        public RoomViewModel()
        {
            RoomCatalog = RoomCatalogSingleton.Instance;
            RoomHandler = new RoomHandler(this);
            _newRoom = new Room();
            CreateRoomCommand = new RelayCommand(RoomHandler.CreateRoom);
            _deleteRoomCommand = new RelayCommand(RoomHandler.DeleteRoom, SelectedIndexIsNotSet);
            _updateRoomCommand = new RelayCommand(RoomHandler.UpdateRoom, SelectedIndexIsNotSet);
            _clearRoomCommand = new RelayCommand(RoomHandler.ClearRoom);
        }

        //Commands

        //Create
        public ICommand CreateRoomCommand { get; }

        //Delete
        private ICommand _deleteRoomCommand;

        public ICommand DeleteRoomCommand
        {
            get { return _deleteRoomCommand; }
            set { _deleteRoomCommand = value; }
        }

        //Update
        private ICommand _updateRoomCommand;

        public ICommand UpdateRoomCommand
        {
            get { return _updateRoomCommand; }
            set { _updateRoomCommand = value; }
        }

        //Clear
        private ICommand _clearRoomCommand;

        public ICommand ClearRoomCommand
        {
            get { return _clearRoomCommand; }
            set { _clearRoomCommand = value; }
        }


        //NewRoom
        private Room _newRoom;
        public Room NewRoom 
        {
            get { return _newRoom; }
            set { _newRoom = value; OnPropertyChanged(); }
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
            }
        }

        public bool SelectedIndexIsNotSet()
        {
            return SelectedIndex != -1;
        }


        private Room _selectedRoom;

        public Room SelectedRoom
        {
            get { return _selectedRoom; }
            set { _selectedRoom = value;
                if (value == null) 
                    _selectedRoom = new Room();
                OnPropertyChanged(); }
        }









        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
