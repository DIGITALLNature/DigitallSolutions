using System;
using D365.Extension.Core;
using D365.Extension.Model;

namespace dgt.solutions.Plugins.Processor
{
    internal class FinalizeProcessor : IWorkbenchProcessor
    {
        private readonly Executor _executor;

        public FinalizeProcessor(Executor executor)
        {
            _executor = executor;
        }

        public ExecutionResult Execute(DgtWorkbench workbench)
        {
            try
            {
                var service = _executor.SecuredOrganizationService;
                _executor.ElevatedOrganizationService.Delete(Solution.EntityLogicalName, Guid.Parse(workbench.DgtSolutionid));
                return ExecutionResult.Ok;
            }
            catch (Exception e)
            {
                _executor.LoggingService.Error(e.RootMessage());
                return ExecutionResult.Failure;
            }
        }
    }
}
