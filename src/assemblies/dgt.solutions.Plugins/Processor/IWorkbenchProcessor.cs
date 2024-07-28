using D365.Extension.Core;
using D365.Extension.Model;

namespace dgt.solutions.Plugins.Processor
{
    public interface IWorkbenchProcessor
    {
        ExecutionResult Execute(DgtWorkbench workbench);
    }
}
