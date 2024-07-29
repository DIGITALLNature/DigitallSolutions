using System;
using System.Collections.Generic;
using System.Linq;
using D365.Extension.Model;
using D365.Extension.Registration;
using dgt.solutions.Plugins.Contract;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace dgt.solutions.Plugins.CarrierConstraints
{
    [CustomApiRegistration(SdkMessageNames.DgtPreventItemsWithoutActiveLayer)]
    public class PreventItemsWithoutActiveLayer : ConstraintBase
    {
        private PicklistAttributeMetadata _componentTypes;

        protected override string ConstraintType => "Prevent ItemsWithouthActiveLayer";

        protected override ConstraintCheckLogEntry RunCheck(Guid solutionId)
        {
            _componentTypes = ((RetrieveAttributeResponse)ElevatedOrganizationService.Execute(
                    new RetrieveAttributeRequest
                    {
                        EntityLogicalName = SolutionComponent.EntityLogicalName,
                        LogicalName = SolutionComponent.LogicalNames.ComponentType
                    })).AttributeMetadata as PicklistAttributeMetadata;
            return CheckForItemsWithouthActiveLayer(solutionId);
        }

        private ConstraintCheckLogEntry CheckForItemsWithouthActiveLayer(Guid solutionId)
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
            var components = GetSolutionComponents(new List<ConditionExpression>
            {
                new ConditionExpression(SolutionComponent.LogicalNames.SolutionId, ConditionOperator.Equal, solutionId),
                new ConditionExpression(SolutionComponent.LogicalNames.ComponentType, ConditionOperator.In,
                    componentsTypes.ToArray())
            });

            var failed = new List<ComponentInfo>();

            foreach (var component in components)
            {
                var layers = GetSolutionLayers(component);
                if (!layers.Any()) continue;

                var first = layers.First();
                if (first.MsdynSolutionname != "Active")
                    failed.Add(new ComponentInfo
                    {
                        ComponentId = component.ObjectId.GetValueOrDefault(),
                        Hint = $"Top Solution: {first.MsdynSolutionname}",
                        ComponentType = GetComponentTypeSetLabel(component.ComponentType.Value),
                    });
            }

            if (failed.Any())
                return new ConstraintCheckLogEntry
                {
                    ConstraintType = "Prevent ItemsWithouthActiveLayer",
                    Succeded = false,
                    ErrorComponents = failed
                };

            return new ConstraintCheckLogEntry
            {
                ConstraintType = "Prevent ItemsWithouthActiveLayer",
                Succeded = true
            };
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
                )
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

        private List<SolutionComponent> GetSolutionComponents(IEnumerable<ConditionExpression> conditions)
        {
            var components = new List<SolutionComponent>();
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
            qe.PageInfo = new PagingInfo
            {
                Count = 5000,
                PageNumber = 1,
                PagingCookie = null
            };
            while (true)
            {
                // Retrieve the page.
                var results = ElevatedOrganizationService.RetrieveMultiple(qe);
                components.AddRange(results.Entities.Select(e => e.ToEntity<SolutionComponent>()));
                if (results.MoreRecords)
                {
                    qe.PageInfo.PageNumber++;
                    qe.PageInfo.PagingCookie = results.PagingCookie;
                }
                else
                {
                    break;
                }
            }

            return components;
        }

        private string GetComponentTypeSetLabel(int value)
        {
            return _componentTypes.OptionSet.Options.Single(o => o.Value == value).Label.UserLocalizedLabel.Label;
        }
    }
}