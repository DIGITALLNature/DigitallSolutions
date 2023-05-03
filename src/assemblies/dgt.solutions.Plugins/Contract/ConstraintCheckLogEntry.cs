using System.Collections.Generic;
using System.Runtime.Serialization;

namespace dgt.solutions.Plugins.Contract
{
    [DataContract]
    public class ConstraintCheckLogEntry
    {
        [DataMember(Name = "Type", Order = 1)]
        public string ConstraintType { get; set; }

        [DataMember(Name = "Succeded", Order = 2)]
        public bool Succeded { get; set; } = true;

        [DataMember(Name = "Errors", Order = 3)]
        public List<ComponentInfo> ErrorComponents { get; set; }
    }
}
