using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLibrary.Exceptions
{
    public class EmptyException : BaseException
    {
        public EmptyException(string message) : base(message)
        {
        }
    }
}
