using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLibrary.Model
{
    class Registration
    {
		private int _id;

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

        public Participant UserpParticipant = new Participant();

		public Event  RegisteredEvent = new Event();

        public Registration()
        {
            
        }

        public Registration(int id, Event _event, Participant participant)
        {
            _id = id;
            RegisteredEvent = _event;
            UserpParticipant = participant;
        }
    }
}
