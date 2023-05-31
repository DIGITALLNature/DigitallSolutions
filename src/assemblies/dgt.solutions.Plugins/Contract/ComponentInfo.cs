using System;
using System.Runtime.Serialization;

namespace dgt.solutions.Plugins.Contract
{
    [DataContract]
    public class ComponentInfo
    {
        [DataMember(Name = "Type", Order = 1)]
        public string ComponentType { get; set; }

        [DataMember(Name = "Id", Order = 2)]
        public Guid ComponentId { get; set; }
        
        [DataMember(Name = "Hint", Order = 3)]
        public string Hint { get; set; }
    }
}