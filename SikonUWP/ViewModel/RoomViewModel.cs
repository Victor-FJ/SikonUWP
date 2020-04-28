using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ModelLibrary.Model;
using SikonUWP.Annotations;
using SikonUWP.Handlers;
using SikonUWP.Model;

namespace SikonUWP.ViewModel
{
    public class RoomViewModel: INotifyPropertyChanged
    {
        public RoomCatalogSingleton RoomCatalogSingleton { get; set; }

        public RoomHandler RoomHandler { get; set; }

        public RoomViewModel()
        {
            RoomCatalogSingleton = RoomCatalogSingleton.Instance;
            RoomHandler = new RoomHandler(this);
            _newRoom = new Room();
        }


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
            set { _selectedIndex = value; OnPropertyChanged();}
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
