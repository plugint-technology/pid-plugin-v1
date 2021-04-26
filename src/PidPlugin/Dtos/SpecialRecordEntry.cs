using System;
using System.Collections.Generic;

namespace PidPlugin.Dtos
{
    public class SpecialRecordEntry
    {
        public string                       Issuer                      { get; set; }
        public string                       Rule                        { get; set; }
        public string                       RuleName                    { get; set; }
        public string                       Category                    { get; set; }
        public DateTime?                    InclusionPublicationDate    { get; set; }
        public DateTime?                    RecordUpdateTime            { get; set; }
        public DateTime?                    SuspensionPublicationDate   { get; set; }
        public DateTime?                    SuspensionRemovalDate       { get; set; }
        public string                       Status                      { get; set; }
        public Dictionary<string, object>   EntityData                  { get; set; }
        public DateTime?                    CreatedAt                   { get; set; }
    }
}
