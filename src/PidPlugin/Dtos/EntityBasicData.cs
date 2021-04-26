using System;
using System.Collections.Generic;

namespace PidPlugin.Dtos
{
    public class EntityBasicData
    {
        public long?                        NaturalKey               { get; set; }
        public string                       KeyType                  { get; set; }
        public string                       DocumentType             { get; set; }
        public string                       PersonType               { get; set; }
        public string                       Name                     { get; set; }
        public string                       LastName                 { get; set; }
        public string                       DocumentNumber           { get; set; }
        public DateTime?                    Birthdate                { get; set; }
        public DateTime?                    Deathdate                { get; set; }
        public string                       KeyStatus                { get; set; }
        public string                       LegalType                { get; set; }
        public DateTime?                    InscriptionDate          { get; set; }
        public string                       AssociatedInactiveKey    { get; set; }
        public int?                         MainActivityId           { get; set; }
        public int?                         MainActivityPeriod       { get; set; }
        public string                       MainActivityDescription  { get; set; }
        public int?                         CloseMonth               { get; set; }
        public DateTime?                    SocialContractDate       { get; set; }
        public string                       BusinessName             { get; set; }
        public string                       Gender                   { get; set; }
        public string                       ResidenceType            { get; set; }
        public string                       InscriptionAuthority     { get; set; }
        public string                       InscriptionNumber        { get; set; }
        public string                       NationalEquityPercentage { get; set; }
        public string                       DisplayName              { get; set; }
        public IEnumerable<Tax>             Taxes                    { get; set; }
        public IEnumerable<TaxesCondition>  TaxesConditions          { get; set; }
        public IEnumerable<Email>           Emails                   { get; set; }
        public IEnumerable<Activity>        Activities               { get; set; }
        public IEnumerable<Telephone>       Telephones               { get; set; }
        public IEnumerable<Address>         Addresses                { get; set; }
    }
}
