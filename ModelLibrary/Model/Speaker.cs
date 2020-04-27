using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;

namespace ModelLibrary.Model
{
    public class Speaker : User
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

		//gemt som string skal implementeres som image
		private string _image;

		public string Image
		{
			get { return _image; }
			set { _image = value; }
		}






		public Speaker()
        {
            
        }
		//constructor mangler image parameter implementeret
        public Speaker(string userName, string password, string fullName, string description) : base(userName, password)
		{
            _fullName = fullName;
            _description = description;
        }
    }
}
