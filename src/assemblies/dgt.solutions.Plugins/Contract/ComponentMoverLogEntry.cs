using System;
using System.Runtime.Serialization;

namespace dgt.solutions.Plugins.Contract
{
    [DataContract]
    public class ComponentMoverLogEntry
    {
        [DataMember(Name = "ComponentId")]
        public Guid ComponentId { get; set; }

        [DataMember(Name = "ComponentType")]
        public string ComponentType { get; set; }

        [DataMember(Name = "RootComponentBehavior")]
        public string RootComponentBehavior { get; set; }
    }
}
