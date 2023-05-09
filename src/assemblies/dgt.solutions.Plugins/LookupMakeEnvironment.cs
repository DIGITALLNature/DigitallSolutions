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
        private const string SolutionConceptMakeEnvironmentId = "dgt_make_enviroment_id";

       
        protected override ExecutionResult Execute()
        {
            var makeEnvironmentId = EnvironmentVariablesService.GetConfig(SolutionConceptMakeEnvironmentId);
            Delegate.TracingService.Trace(makeEnvironmentId);
            SetOutputParameter("MakeEnvironmentId", makeEnvironmentId);
            return ExecutionResult.Ok;
        }
    }
}
