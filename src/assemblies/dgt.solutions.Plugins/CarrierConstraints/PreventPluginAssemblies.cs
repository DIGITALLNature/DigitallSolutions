using System;
using System.Collections.Generic;
using System.Linq;
using D365.Extension.Model;
using D365.Extension.Registration;
using dgt.solutions.Plugins.Helper;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace dgt.solutions.Plugins.CarrierConstraints
{
    [CustomApiRegistration(SdkMessageNames.DgtPreventPluginAssemblies)]
    public class PreventPluginAssemblies : ConstraintBase
    {
        private PicklistAttributeMetadata _componentTypes;

        protected override string ConstraintType => "Prevent PreventPluginAssemblys";

        protected override bool RunCheck(Guid originId)
        {
            var componentsTypes = new List<int>
            {
                SolutionComponent.Options.ComponentType.PluginAssembly,
                SolutionComponent.Options.ComponentType.SDKMessageProcessingStep
            };
            var components = GetSolutionComponents(new List<ConditionExpression>
            {
                new ConditionExpression(SolutionComponent.LogicalNames.SolutionId, ConditionOperator.Equal, originId),
                new ConditionExpression(SolutionComponent.LogicalNames.ComponentType, ConditionOperator.In,
                    componentsTypes.ToArray())
            });

            if (components.Any())
            {
                components.ForEach(c => WorkbenchHistoryLogger?.LogConstraintViolation(ConstraintType, "PluginAssembly", c.ObjectId));
                return false;
            }

            WorkbenchHistoryLogger?.LogConstraintSuccess(ConstraintType);
            return true;
        }

        private List<SolutionComponent> GetSolutionComponents(IEnumerable<ConditionExpression> conditions)
        {
            var qe = new QueryExpression(SolutionComponent.EntityLogicalName)
            {
                NoLock = true,
                ColumnSet = new ColumnSet(true)
            };
            qe.Criteria.Conditions.AddRange(conditions);
            qe.Orders.Add(new OrderExpression
            {
                AttributeName = SolutionComponent.LogicalNames.SolutionComponentId,
                OrderType = OrderType.Ascending
            });

            return ElevatedOrganizationService.RetrieveMultiplePaged<SolutionComponent>(qe, Delegate.TracingService).ToList();
        }

        private string GetComponentTypeSetLabel(int value)
        {
            return _componentTypes.OptionSet.Options.Single(o => o.Value == value).Label.UserLocalizedLabel.Label;
        }

    }
}
