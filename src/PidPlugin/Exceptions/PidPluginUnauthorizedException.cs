using System;

namespace PidPlugin.Exceptions
{
    public class PidPluginUnauthorizedException : Exception
    {
        public PidPluginUnauthorizedException()
            : base("Unauthorized")
        {
            //
        }
    }
}
