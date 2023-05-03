using System;

namespace dgt.solutions.Plugins.Helper
{
    internal class ConstraintViolationException : Exception
    {
        public ConstraintViolationException(string message) : base(message)
        {

        }
    }
}
