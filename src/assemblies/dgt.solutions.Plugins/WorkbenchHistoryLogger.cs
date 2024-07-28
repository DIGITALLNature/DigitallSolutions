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

        public static WorkbenchHistoryLogger Create(IOrganizationService orgService, DgtWorkbench workbench)
        {
            var workbenchHistory = new DgtWorkbenchHistory { DgtWorkbenchId = workbench.ToEntityReference(), };
            workbenchHistory.Id = orgService.Create(workbenchHistory);

            return new WorkbenchHistoryLogger(orgService, workbenchHistory);
        }
    }
}