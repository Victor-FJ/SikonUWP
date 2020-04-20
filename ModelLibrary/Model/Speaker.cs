using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;

namespace ModelLibrary.Model
{
    public class Speaker
    {
		private string _fullName;

		public string FullName
		{
			get { return _fullName; }
			set { _fullName = value; }
		}

		private string _description;

		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

        //public Image image;





        public Speaker()
        {
            
        }
		//constructor mangler image parameter implementeret
        public Speaker(string fullName, string description)
        {
            _fullName = fullName;
            _description = description;
        }
    }
}
