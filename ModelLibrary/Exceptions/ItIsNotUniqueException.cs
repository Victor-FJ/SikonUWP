using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLibrary.Exceptions
{
    public class ItIsNotUniqueException: BaseException
    {
        public ItIsNotUniqueException(string message) : base(message)
        {

        }
    }
}
