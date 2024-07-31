using System;
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

        public void LogDebug(string message, string subType = default, string componentType = default, Guid? objectId = default)
        {
            _orgService.Create(new DgtWorkbenchHistoryLog
            {
                DgtComponentType = componentType,
                DgtSubtypeTxt = subType,
                DgtMessage = message,
                DgtTypeSet = new OptionSetValue(DgtWorkbenchHistoryLog.Options.DgtTypeSet.Log),
                DgtLogLevelSet = new OptionSetValue(DgtWorkbenchHistoryLog.Options.DgtLogLevelSet._3Debug),
                DgtWorkbenchHistoryId = WorkbenchHistory.ToEntityReference(),
                DgtObjectidTxt = objectId.ToString(),
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
                DgtObjectidTxt = component.ObjectId.GetValueOrDefault().ToString(),
            });
        }

        internal void LogConstraintViolation(string constraintType, string componentType, Guid? objectId, string message = default)
        {
            _orgService.Create(new DgtWorkbenchHistoryLog
            {
                DgtLogLevelSet = new OptionSetValue(DgtWorkbenchHistoryLog.Options.DgtLogLevelSet._0Error),
                DgtComponentType = componentType,
                DgtMessage = message ?? "Failed",
                DgtTypeSet = new OptionSetValue(DgtWorkbenchHistoryLog.Options.DgtTypeSet.Constraint),
                DgtSubtypeTxt = constraintType,
                DgtWorkbenchHistoryId = WorkbenchHistory.ToEntityReference(),
                DgtObjectidTxt = objectId?.ToString(),
            });
        }

        internal void LogConstraintSuccess(string constraintType)
        {
            _orgService.Create(new DgtWorkbenchHistoryLog
            {
                DgtLogLevelSet = new OptionSetValue(DgtWorkbenchHistoryLog.Options.DgtLogLevelSet._2Information),
                DgtComponentType = "workflow",
                DgtMessage = "Succeeded",
                DgtTypeSet = new OptionSetValue(DgtWorkbenchHistoryLog.Options.DgtTypeSet.Constraint),
                DgtSubtypeTxt = constraintType,
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