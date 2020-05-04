using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLibrary.Exceptions
{
    public class BaseException : Exception
    {
        public BaseException(string message) : base(message)
        {
        }
    }
}
