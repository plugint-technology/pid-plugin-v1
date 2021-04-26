namespace PidPlugin.Dtos
{
    public class Address
    {
        public string   Description             { get; set; }
        public string   ZipCode                 { get; set; }
        public string   State                   { get; set; }
        public int?     StateId                 { get; set; }
        public string   Street                  { get; set; }
        public int?     Number                  { get; set; }
        public string   Floor                   { get; set; }
        public string   Block                   { get; set; }
        public string   Sector                  { get; set; }
        public string   Tower                   { get; set; }
        public string   Line1                   { get; set; }
        public string   City                    { get; set; }
        public string   AditionalDataType       { get; set; }
        public string   LocalDepartmentOffice   { get; set; }
        public string   AddressStatus           { get; set; }
        public string   AddressTypeAux          { get; set; }
    }
}
