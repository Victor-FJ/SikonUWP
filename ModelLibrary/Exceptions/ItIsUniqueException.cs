using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLibrary.Exceptions
{
    public class ItIsUniqueException : BaseException
    {
        public ItIsUniqueException(string message) : base(message)
        {
        }
    }
}
