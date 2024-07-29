using D365.Extension.Model;
using Microsoft.Xrm.Sdk;

namespace dgt.solutions.Plugins
{
    public class WorkbenchHistoryLogger
    {
        public DgtWorkbenchHistory WorkbenchHistory { get; private set; }

        private readonly IOrganizationService _orgService;

        public WorkbenchHistoryLogger(IOrganizationService orgService, DgtWorkbenchHistory workbenchHistory)
        {
            _orgService = orgService;
            WorkbenchHistory = workbenchHistory;
        }

        public void LogDebug(OptionSetValue type, string message)
        {
            _orgService.Create(new DgtWorkbenchHistoryLog
            {
                DgtMessage = message,
                DgtTypeSet = type,
                DgtWorkbenchHistoryId = WorkbenchHistory.ToEntityReference(),
            });
        }

        public void LogComponent(SolutionComponent component)
        {
            component.FormattedValues.TryGetValue(SolutionComponent.LogicalNames.ComponentType, out var componentType);
            _orgService.Create(new DgtWorkbenchHistoryLog
            {
                DgtComponentType = componentType ?? $"ComponentType: {component.ComponentType}",
                DgtMessage = component.ObjectId.GetValueOrDefault().ToString(),
                DgtTypeSet = new OptionSetValue(DgtWorkbenchHistoryLog.Options.DgtTypeSet.ComponentMove),
                DgtWorkbenchHistoryId = WorkbenchHistory.ToEntityReference(),
            });
        }

        public static WorkbenchHistoryLogger Create(IOrganizationService orgService, DgtWorkbench workbench, string message)
        {
            var workbenchHistory = new DgtWorkbenchHistory
            {
                DgtEntry = message,
                DgtWorkbenchId = workbench.ToEntityReference(),
            };
            workbenchHistory.Id = orgService.Create(workbenchHistory);

            return new WorkbenchHistoryLogger(orgService, workbenchHistory);
        }
    }
}