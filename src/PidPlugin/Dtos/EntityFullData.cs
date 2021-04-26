using System.Collections.Generic;

namespace PidPlugin.Dtos
{
    public class EntityFullData : EntityBasicData
    {
        public IEnumerable<Exclusion>           Exclusions              { get; set; }
        public IEnumerable<SpecialRecordEntry>  SpecialRecordEntries    { get; set; }
    }
}
