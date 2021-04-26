namespace PidPlugin.Dtos
{
    public class Account
    {
        public string   type            { get; set; }
        public string   currency        { get; set; }
        public string   bank            { get; set; }
        public string   alias           { get; set; }
        public bool?    active          { get; set; }
        public string   ownerName       { get; set; }
        public string   ownerKeyType    { get; set; }
        public string   ownerIsHuman    { get; set; }
    }
}
