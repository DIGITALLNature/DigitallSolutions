using D365.Extension.Core;
using D365.Extension.Model;
using D365.Extension.Registration;
using dgt.solutions.Plugins.Helper;

namespace dgt.solutions.Plugins
{
    [CustomApiRegistration(SdkMessageNames.DgtSynchronizeAssemblies)]
    public class SynchronizeAssembliesService : Executor
    {
        protected override ExecutionResult Execute()
        {
            GetInputParameter("Origin", out string origin);
            GetInputParameter("Destination", out string destination);

            Delegate.TracingService.Trace(origin);
            Delegate.TracingService.Trace(destination);

            var solutionLookup = new SolutionLookup(this);
            var originSolution = solutionLookup.GetByName(origin.Trim());
            var destinationSolution = solutionLookup.GetByName(destination.Trim());

            new ComponentMover(this).MoveAssemblyComponents(originSolution.Id, destinationSolution.UniqueName);
            return ExecutionResult.Ok;
        }
    }
}
