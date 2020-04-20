using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLibrary.Model
{
    class Participant
    {
        public enum PersonType
        {
            Autist, ForældreAfAutist, Psykolog, Fagperson, Studerende
        }

        public PersonType Type;



        public Participant()
        {
            
        }

        public Participant(PersonType personType)
        {
            Type = personType;
        }
    }
}
