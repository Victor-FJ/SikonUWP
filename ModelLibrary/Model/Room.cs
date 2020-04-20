using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLibrary.Model
{
    public class Room
    {
		private string _roomNo;

		public string RoomNo
		{
			get { return _roomNo; }
			set { _roomNo = value; }
		}

		private string _locationDescription;

		public string LocationDescription
		{
			get { return _locationDescription; }
			set { _locationDescription = value; }
		}

		private int _maxNoPeople;

		public int MaxNoPeople
		{
			get { return _maxNoPeople; }
			set { _maxNoPeople = value; }
		}



        public Room()
        {
            
        }


        public Room(string roomNo, string locationDescription, int maxNoPeople)
        {
            _roomNo = roomNo;
            _locationDescription = locationDescription;
            _maxNoPeople = maxNoPeople;
        }


	}
}
