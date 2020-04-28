using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SikonUWP.Persistency;
using ModelLibrary.Model;
using SikonUWP.Model;

namespace SikonUWP.ViewModel
{
    public class EventEditorViewModel
    {
        public Event EditedEvent { get; set; }

        #region ControlProperties

        public string Title
        {
            get { return EditedEvent.Title; }
            set
            {
                
            }
        }


        #endregion

        public EventEditorViewModel()
        {
            if (EventCatalogSingleton.Instance.MarkedEvent == null)
                EventCatalogSingleton.Instance.MarkedEvent = new Event();
            EditedEvent = EventCatalogSingleton.Instance.MarkedEvent;
        }
    }
}
