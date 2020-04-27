using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ModelLibrary.Model
{
    public class Event
    {
        #region Enums

        public enum EventType
        {
            Plenum,
            Tema,
            Workshop,
            Marked,
            Konkurrence,
            Forplejning
        }

        public enum EventSubject
        {
            Autisme,
            PædagogiskUdvikling,
            WakeUp
        }

        #endregion

        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public EventType Type { get; set; }

        public EventSubject Subject { get; set; }

        public int MaxNoParticipant { get; set; }

        public DateTimeOffset Date { get; set; }

        public TimeSpan Time { get; set; }

        public User Speaker { get; set; }

        public Room Room { get; set; }

        public string ImageName { get; set; }
    }
}
