using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using ModelLibrary.Exceptions;

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

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new EmptyException("Titlen kan ikke være tom");
                if (value.Length > 100)
                    throw new OutsideRangeException("Titlen kan ikke være større end 100 karaktere");
                _title = value;
            }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new EmptyException("Beskrivelsen kan ikke være tom");
                if (value.Length > 3000)
                    throw new OutsideRangeException("Beskrivelsen kan ikke være større end 3000 karaktere");
                _description = value;
            }
        }

        public EventType Type { get; set; }

        public EventSubject Subject { get; set; }

        private int _maxNoParticipant;
        public int MaxNoParticipant
        {
            get => _maxNoParticipant;
            set
            {
                if (value < 0)
                    throw new OutsideRangeException("Max antal deltagere kan ikke være mindre end 0");
                if (Room != null && value > Room.MaxNoPeople)
                    throw new OutsideRangeException("Max antal deltagere er højere en rummet kan holde");
                _maxNoParticipant = value;
            }
        }

        private DateTimeOffset _startDate;
        public DateTimeOffset StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                if (_endDate < _startDate)
                    _endDate = _startDate.AddHours(2);
            }
        }

        private DateTimeOffset _endDate;
        public DateTimeOffset EndDate
        {
            get => _endDate;
            set
            {
                if (value < StartDate)
                    throw new OutsideRangeException("Slut Datoen kan ikke ligge før Start datoen");
                _endDate = value;
            }
        }

        private Room _room;
        public Room Room
        {
            get => _room;
            set
            {
                _room = value;
                if (_room != null && _room.MaxNoPeople < _maxNoParticipant)
                    _maxNoParticipant = _room.MaxNoPeople;
            }
        }

        public Speaker Speaker { get; set; }

        private string _imageName;
        public string ImageName
        {
            get => _imageName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new EmptyException("Billed navn kan ikke være tom");
                if (value.Length > 50)
                    throw new OutsideRangeException("Billed navn kan ikke være større end 50 karaktere");
                _imageName = value;
            }
        }


        public Event()
        {
            _title = "Title";
            _description = "Description";
            _startDate = DateTimeOffset.Now.AddDays(2);
            _endDate = DateTimeOffset.Now.AddHours(50);
            _imageName = "Billed navn";
        }

        public Event(int id, string title, string description, EventType type, EventSubject subject, int maxNoParticipant, DateTimeOffset startDate, DateTimeOffset endDate, Room room, Speaker speaker, string imageName)
        {
            Id = id;
            Title = title;
            Description = description;
            Type = type;
            Subject = subject;
            MaxNoParticipant = maxNoParticipant;
            StartDate = startDate;
            EndDate = endDate;
            Speaker = speaker;
            Room = room;
            ImageName = imageName;
        }

        public override string ToString()
        {
            return $"No. {Id} - {Title}";
        }
    }
}
