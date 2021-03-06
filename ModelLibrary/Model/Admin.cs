﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLibrary.Model
{
    public class Admin : User
    {
		private string _phoneNumber;

        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set { _phoneNumber = value; }
        }

        public Admin() : base()
        {
            
        }

        public Admin(string userName, string password, string phoneNumber) : base(userName, password)
        {
            _phoneNumber = phoneNumber;
        }

	}
}
