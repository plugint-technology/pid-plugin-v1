using System.Collections.Generic;

namespace PidPlugin.Dtos
{
    public class BankAccountDetail
    {
        public IEnumerable<Owner>   owners          { get; set; }
        public string               type            { get; set; }
        public bool?                is_active       { get; set; }
        public string               currency        { get; set; }
        public string               label           { get; set; }
        public Account_Routing      account_routing { get; set; }
        public Bank_Routing         bank_routing    { get; set; }
    }
}
