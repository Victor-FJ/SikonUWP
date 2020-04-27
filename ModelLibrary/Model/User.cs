using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLibrary.Model
{
    public class User
    {
		private string _userName;

		public string UserName
		{
			get { return _userName; }
			set { _userName = value; }
		}

		private string  _password;

		public string  Password
		{
			get { return _password; }
			set { _password = value; }
		}



        public User()
        {
            
        }


        public User(string userName, string password)
        {
            _userName = userName;
            _password = password;
        }

    }
}
