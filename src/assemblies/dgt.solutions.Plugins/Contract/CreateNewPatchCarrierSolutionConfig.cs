using System.Runtime.Serialization;

namespace dgt.solutions.Plugins.Contract
{
    [DataContract]
    public class CreateNewPatchCarrierSolutionConfig
    {
        //build or revision
        [DataMember(Name = "PatchPattern")]
        public string PatchPattern { get; set; }
    }
}
