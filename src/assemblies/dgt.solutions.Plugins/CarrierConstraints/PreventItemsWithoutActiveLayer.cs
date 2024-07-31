using System;
using System.Collections.Generic;
using System.Linq;
using D365.Extension.Model;
using D365.Extension.Registration;
using dgt.solutions.Plugins.Helper;
using Microsoft.Xrm.Sdk.Query;

namespace dgt.solutions.Plugins.CarrierConstraints
{
    [CustomApiRegistration(SdkMessageNames.DgtPreventItemsWithoutActiveLayer)]
    public class PreventItemsWithoutActiveLayer : ConstraintBase
    {
        protected override string ConstraintType => "Prevent ItemsWithouthActiveLayer";

        protected override bool RunCheck(Guid solutionId)
        {
            Delegate.TracingService.Trace("loading components");
            var components = GetSolutionComponents(solutionId);

            var passed = true;
            foreach (var component in components)
            {
                Delegate.TracingService.Trace("{componentId}: loading layers for component", component.Id);
                var layers = GetSolutionLayers(component);
                if (!layers.Any())
                {
                    Delegate.TracingService.Trace("{componentId}: no layers found", component.Id);
                    continue;
                }

                var first = layers.First();
                component.FormattedValues.TryGetValue(SolutionComponent.LogicalNames.ComponentType, out var componentType);
                if (first.MsdynSolutionname != "Active")
                {
                    Delegate.TracingService.Trace("{componentId}: failed - top solution '{solution}'", component.Id, first.MsdynSolutionname);
                    WorkbenchHistoryLogger?.LogConstraintViolation(ConstraintType, componentType, component.ObjectId, $"Top Solution: {first.MsdynSolutionname}");
                    passed = false;
                }
                else
                {
                    Delegate.TracingService.Trace("{componentId}: passed", component.Id);
                    WorkbenchHistoryLogger?.LogDebug("Top Solution: Active", ConstraintType, componentType, component.ObjectId);
                }
            }

            if (!passed) return false;

            WorkbenchHistoryLogger?.LogConstraintSuccess(ConstraintType);
            return true;
        }

        protected List<MsdynComponentlayer> GetSolutionLayers(SolutionComponent component)
        {
            var query = new QueryExpression
            {
                EntityName = MsdynComponentlayer.EntityLogicalName,
                NoLock = true,
                ColumnSet = new ColumnSet(
                    MsdynComponentlayer.LogicalNames.MsdynName,
                    MsdynComponentlayer.LogicalNames.MsdynSolutionname,
                    MsdynComponentlayer.LogicalNames.MsdynOrder
                ),
                TopCount = 1,
            };
            var filter = new FilterExpression(LogicalOperator.And);
            filter.Conditions.Add(
                new ConditionExpression
                {
                    AttributeName = MsdynComponentlayer.LogicalNames.MsdynComponentid,
                    Operator = ConditionOperator.Equal,
                    Values = { $"{component.ObjectId:B}" }
                }
            );
            query.Criteria = filter;

            query.AddOrder(MsdynComponentlayer.LogicalNames.MsdynOrder, OrderType.Descending);
            var layers = ElevatedOrganizationService.RetrieveMultiple(query).Entities
                .Select(s => s.ToEntity<MsdynComponentlayer>()).ToList();
            return layers;
        }

        private List<SolutionComponent> GetSolutionComponents(Guid solutionId)
        {
            var componentsTypes = new List<int>
            {
                SolutionComponent.Options.ComponentType.Attribute,
                SolutionComponent.Options.ComponentType.Form,
                SolutionComponent.Options.ComponentType.OptionSet,
                SolutionComponent.Options.ComponentType.Privilege,
                SolutionComponent.Options.ComponentType.CanvasApp,
                SolutionComponent.Options.ComponentType.SiteMap,
                SolutionComponent.Options.ComponentType.ViewAttribute,
                SolutionComponent.Options.ComponentType.SystemForm
            };
            var qe = new QueryExpression(SolutionComponent.EntityLogicalName)
            {
                NoLock = true,
                ColumnSet = new ColumnSet(true),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression(SolutionComponent.LogicalNames.SolutionId, ConditionOperator.Equal, solutionId),
                        new ConditionExpression(SolutionComponent.LogicalNames.ComponentType, ConditionOperator.In, componentsTypes.ToArray())
                    }
                }
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
