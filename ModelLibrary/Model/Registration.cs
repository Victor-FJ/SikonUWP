using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLibrary.Model
{
    public class Registration
    {
        public int Id { get; }

        public string UserName { get; }

        public int EventId { get; }

        public Registration(int id, string userName, int eventId)
        {
            Id = id;
            UserName = userName;
            EventId = eventId;
        }
    }
}
