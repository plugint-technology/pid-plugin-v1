using System;

namespace PidPlugin.Cache
{
    public class CacheOptions
    {
        public TimeSpan EntityDataBasicExpiration       { get; set; }
        public TimeSpan EntityDataFullExpiration        { get; set; }
        public TimeSpan SpecialRecordsExpiration        { get; set; }
        public TimeSpan BankAccountDetailExpiration     { get; set; }
        public TimeSpan BankAccountOwnershipExpiration  { get; set; }
    }
}
