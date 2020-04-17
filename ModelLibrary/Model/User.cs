using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLibrary.Model
{
    class User
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

        public object _usertype;

        public object Usertype
        {
            get { return _usertype;}
            set { _usertype = value; }
        }



        public User()
        {
            
        }


        public User(string userName, string password, object usertype)
        {
            _userName = userName;
            _password = password;
            Usertype = usertype;
        }

    }
}
