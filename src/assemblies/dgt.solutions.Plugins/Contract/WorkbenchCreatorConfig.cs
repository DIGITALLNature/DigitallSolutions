using System.Runtime.Serialization;

namespace dgt.solutions.Plugins.Contract
{
    [DataContract]
    public class WorkbenchCreatorConfig
    {
        [DataMember(Name = "Publisher")]
        public string Publisher { get; set; }
    }
}
