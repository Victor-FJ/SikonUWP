using System;
using System.Collections.Generic;
using System.Text;


namespace ModelLibrary.Model
{
    public class Event
    {
		private int _id;

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		private string _title;

		public string Title
		{
			get { return _title; }
			set { _title = value; }
		}

		private string _description;

		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		private DateTime _date;

		public DateTime Date
		{
			get { return _date; }
            set { _date = value; }
        }

		private TimeSpan _time;

		public TimeSpan Time
		{
			get { return _time; }
			set { _time = value; }
		}



		//image mangler at blive implementeret ordenligt
        //public BitmapImage image;

		public Room Room = new Room();





        public Event()
        {
            
        }


        //image parameter mangler at blive implementeret ordenligt
		public Event(int id, string title, string description, DateTime date, TimeSpan time, Room room)
        {
            _id = id;
            _title = title;
            _description = description;
            _date = date;
            _time = time;
            Room = room;
        }
	}
}
