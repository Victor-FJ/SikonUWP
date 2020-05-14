using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLibrary.Model
{
    public class Participant : User
    {
        public enum PersonType
        {
            Vælg_type, Autist, ForældreAfAutist, Psykolog, Fagperson, Studerende
        }

        public PersonType Type { get; set; }



        public Participant()
        {
            
        }

        public Participant(string userName, string password, PersonType personType) : base(userName, password)
        {
            Type = personType;
        }
    }
}
