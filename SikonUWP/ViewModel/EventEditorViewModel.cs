using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SikonUWP.Persistency;
using ModelLibrary.Model;

namespace SikonUWP.ViewModel
{
    public class EventEditorViewModel
    {
        public void Hej()
        {
            GenericPersistence<Event, int> hej = new GenericPersistence<Event, int>("");
        }
    }
}
