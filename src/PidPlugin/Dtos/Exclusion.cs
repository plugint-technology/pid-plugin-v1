using System;

namespace PidPlugin.Dtos
{
    public class Exclusion
    {
        public string       Issuer              { get; set; }
        public string       Rule                { get; set; }
        public string       RuleName            { get; set; }
        public DateTime?    IssueDate           { get; set; }
        public DateTime?    EndDate             { get; set; }
        public double?      Percentage          { get; set; }
        public string       CertificationNumber { get; set; }
        public DateTime?    CreatedAt           { get; set; }
        public string       FiscalPeriod        { get; set; }
    }
}
