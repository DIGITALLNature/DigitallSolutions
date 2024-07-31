using System;
using System.Collections.Generic;
using System.Linq;
using D365.Extension.Model;
using D365.Extension.Registration;
using dgt.solutions.Plugins.Helper;
using Microsoft.Xrm.Sdk.Query;

namespace dgt.solutions.Plugins.CarrierConstraints
{
    [CustomApiRegistration(SdkMessageNames.DgtPreventPluginAssemblies)]
    public class PreventPluginAssemblies : ConstraintBase
    {
        protected override string ConstraintType => "Prevent PreventPluginAssemblys";

        protected override bool RunCheck(Guid solutionId)
        {
            Delegate.TracingService.Trace("loading components");
            var components = GetSolutionComponents(solutionId);

            if (components.Any())
            {
                Delegate.TracingService.Trace("constraint failed");
                components.ForEach(c => WorkbenchHistoryLogger?.LogConstraintViolation(ConstraintType, "PluginAssembly", c.ObjectId));
                return false;
            }

            Delegate.TracingService.Trace("constraint passed");
            WorkbenchHistoryLogger?.LogConstraintSuccess(ConstraintType);
            return true;
        }

        private List<SolutionComponent> GetSolutionComponents(Guid solutionId)
        {
            var componentsTypes = new List<int>
            {
                SolutionComponent.Options.ComponentType.PluginAssembly,
                SolutionComponent.Options.ComponentType.SDKMessageProcessingStep
            };
            var qe = new QueryExpression(SolutionComponent.EntityLogicalName)
            {
                NoLock = true,
                ColumnSet = new ColumnSet(true),
                Criteria = {
                    Conditions = {
                        new ConditionExpression(SolutionComponent.LogicalNames.SolutionId, ConditionOperator.Equal, solutionId),
                        new ConditionExpression(SolutionComponent.LogicalNames.ComponentType, ConditionOperator.In, componentsTypes.ToArray()),
                    },
                },
            };
            qe.Orders.Add(new OrderExpression
            {
                AttributeName = SolutionComponent.LogicalNames.SolutionComponentId,
                OrderType = OrderType.Ascending
            });

            return ElevatedOrganizationService.RetrieveMultiplePaged<SolutionComponent>(qe, Delegate.TracingService).ToList();
        }
    }
}
