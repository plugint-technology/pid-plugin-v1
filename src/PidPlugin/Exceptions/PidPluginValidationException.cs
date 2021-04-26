using System;

namespace PidPlugin.Exceptions
{
    public class PidPluginValidationException : Exception
    {
        public PidPluginValidationException(string message)
            : base(message)
        {
            //
        }
    }
}
