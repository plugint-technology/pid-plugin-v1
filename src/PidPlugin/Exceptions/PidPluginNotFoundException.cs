using System;

namespace PidPlugin.Exceptions
{
    public class PidPluginNotFoundException : Exception
    {
        public PidPluginNotFoundException()
            : base("Resource not found")
        {
            //
        }
    }
}
