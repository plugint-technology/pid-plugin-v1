using System;

namespace PidPlugin.Cache
{
    public interface ICacheAccessor
    {
        bool TryGetValue    (string key, out object value);
        void Set            (string key, object value, TimeSpan expirationTime);
    }
}
