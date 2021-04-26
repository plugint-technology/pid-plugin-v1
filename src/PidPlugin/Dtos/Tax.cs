namespace PidPlugin.Dtos
{
    public class Tax
    {
        public string   Description     { get; set; }
        public string   DaysInPeriod    { get; set; }
        public int?     TaxId           { get; set; }
        public int?     Period          { get; set; }
        public string   Status          { get; set; }
        public string   InscriptionDate { get; set; }
    }
}
