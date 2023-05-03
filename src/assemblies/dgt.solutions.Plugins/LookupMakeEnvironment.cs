using System;
using D365.Extension.Core;
using D365.Extension.Model;
using D365.Extension.Registration;
using dgt.solutions.Plugins.Contract;

namespace dgt.solutions.Plugins
{
    [CustomApiRegistration(SdkMessageNames.DgtLookupMakeEnvironment)]
    public class LookupMakeEnvironment : Executor
    {
        private const string SolutionConceptMakeEnvironmentId = "SolutionConcept.MakeEnvironmentId";

        public LookupMakeEnvironment(string unsecure, string secure) : base(unsecure, secure)
        {
        }

        public override string GetConfig(string key, int lcid = 1033, string defaultValue = null)
        {
            switch (key)
            {
                case SolutionConceptMakeEnvironmentId:
                    //null for default or { "MakeEnvironmentId": "7a28c553-78bc-4866-bad9-edfdb2538201" }
                    return string.IsNullOrWhiteSpace(SecureConfig) ? Guid.Empty.ToString("D") : SerializerService.JsonDeserialize<LookupMakeEnvironmentConfig>(SecureConfig).MakeEnvironmentId;
                default:
                    return defaultValue;
            }
        }

        protected override ExecutionResult Execute()
        {
            var makeEnvironmentId = ConfigService.GetConfig(SolutionConceptMakeEnvironmentId);
            Delegate.TracingService.Trace(makeEnvironmentId);
            SetOutputParameter("MakeEnvironmentId", makeEnvironmentId);
            return ExecutionResult.Ok;
        }
    }
}
