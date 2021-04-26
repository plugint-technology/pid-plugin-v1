namespace PidPlugin.Settings
{
    public class PidPluginSettings
    {
        public string        BaseUrl                  { get; set; }
        public int           TimeoutInMinutes         { get; set; }
        public string        SubscriptionKey          { get; set; }
        public int           RetryAttempts            { get; set; }
        public CacheSettings CacheSettings            { get; set; }
    }

    public class CacheSettings
    {
        public int EntityDataBasicInMinutes      { get; set; }
        public int EntityDataFullInMinutes       { get; set; }
        public int SpecialRecordsInMinutes       { get; set; }
        public int BankAccountDetailInMinutes    { get; set; }
        public int BankAccountOwnershipInMinutes { get; set; }
    }
}
