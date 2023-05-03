using System.Runtime.Serialization;

namespace dgt.solutions.Plugins.Contract
{
    [DataContract]
    public class MergeAllPatchCarrierSolutionConfig
    {
        //major or minor
        [DataMember(Name = "PatchPattern")]
        public string PatchPattern { get; set; }
    }
}
