using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLibrary.Exceptions
{
    public class OutsideRangeException : BaseException
    {
        public OutsideRangeException(string message) : base(message)
        {
        }
    }
}
