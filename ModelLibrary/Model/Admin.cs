using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLibrary.Model
{
    public class Admin
    {
		private string _phoneNumber;

		public string PhoneNumber			
		{
			get { return _phoneNumber; }
			set { _phoneNumber = value; }
		}



        public Admin()
        {
            
        }
        public Admin(string phoneNumber)
        {
            _phoneNumber = phoneNumber;
        }

	}
}
