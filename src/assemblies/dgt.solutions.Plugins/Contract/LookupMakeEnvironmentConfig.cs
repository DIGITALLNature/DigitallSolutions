using System.Runtime.Serialization;

namespace dgt.solutions.Plugins.Contract
{
    [DataContract]
    public class LookupMakeEnvironmentConfig
    {
        [DataMember(Name = "MakeEnvironmentId")]
        public string MakeEnvironmentId { get; set; }
    }
}
