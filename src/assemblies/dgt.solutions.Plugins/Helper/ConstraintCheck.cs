using System;
using System.Collections.Generic;
using System.Linq;
using D365.Extension.Core;
using D365.Extension.Model;
using dgt.solutions.Plugins.Contract;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace dgt.solutions.Plugins.Helper
{
    internal class ConstraintCheck
    {
        private readonly PicklistAttributeMetadata _componentTypes;
        private readonly Executor _executor;

        public ConstraintCheck(Executor executor)
        {
            _executor = executor;
            _componentTypes = ((RetrieveAttributeResponse)executor.ElevatedOrganizationService.Execute(
                new RetrieveAttributeRequest
                {
                    EntityLogicalName = SolutionComponent.EntityLogicalName,
                    LogicalName = SolutionComponent.LogicalNames.ComponentType
                })).AttributeMetadata as PicklistAttributeMetadata;
        }

        internal IEnumerable<ConstraintCheckLogEntry> Check(DgtCarrier carrier, DgtWorkbench workbench)
        {
            if (carrier.DgtConstraintMset == null)
                yield return new ConstraintCheckLogEntry
                {
                    ConstraintType = "No constraints defined"
                };
            else
                foreach (var constraint in carrier.DgtConstraintMset)
                    switch (constraint.Value)
                    {
                        case DgtCarrier.Options.DgtConstraintMset.PreventFlows:
                            yield return CheckForFlows(Guid.Parse(workbench.DgtSolutionid));
                            break;
                        case DgtCarrier.Options.DgtConstraintMset.PreventItemsWithouthActiveLayer:
                            yield return CheckForItemsWithouthActiveLayer(Guid.Parse(workbench.DgtSolutionid));
                            break;
                        case DgtCarrier.Options.DgtConstraintMset.PreventManagedEntitiesWithAllAssets:
                            yield return CheckForManagedEntitiesWithAllAssets(Guid.Parse(workbench.DgtSolutionid));
                            break;
                        case DgtCarrier.Options.DgtConstraintMset.PreventPluginAssemblys:
                            yield return CheckForAssemblysAndAssemblysSteps(Guid.Parse(workbench.DgtSolutionid));
                            break;
                    }
        }

        private ConstraintCheckLogEntry CheckForAssemblysAndAssemblysSteps(Guid originId)
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
                return new ConstraintCheckLogEntry
                {
                    ConstraintType = "Prevent PreventPluginAssemblys",
                    Succeded = false,
                    ErrorComponents = components.Select(c => new ComponentInfo
                    {
                        ComponentId = c.ObjectId.GetValueOrDefault(),
                        ComponentType = GetComponentTypeSetLabel(c.ComponentType.Value)
                    }).ToList()
                };
            return new ConstraintCheckLogEntry
            {
                ConstraintType = "Prevent PreventPluginAssemblys"
            };
        }

        private string GetComponentTypeSetLabel(int value)
        {
            return _componentTypes.OptionSet.Options.Single(o => o.Value == value).Label.UserLocalizedLabel.Label;
        }

        private ConstraintCheckLogEntry CheckForManagedEntitiesWithAllAssets(Guid originId)
        {
            var components = GetSolutionComponents(new List<ConditionExpression>
            {
                new ConditionExpression(SolutionComponent.LogicalNames.SolutionId, ConditionOperator.Equal, originId),
                new ConditionExpression(SolutionComponent.LogicalNames.ComponentType, ConditionOperator.Equal,
                    SolutionComponent.Options.ComponentType.Entity)
            });

            var request = new RetrieveAllEntitiesRequest
            {
                EntityFilters = EntityFilters.Entity
            };
            var response = (RetrieveAllEntitiesResponse)_executor.ElevatedOrganizationService.Execute(request);
            var entities = response.EntityMetadata.ToList();
            var failedEntities = new List<ComponentInfo>();

            foreach (var component in components.Where(c =>
                         c.RootComponentBehavior?.Value ==
                         SolutionComponent.Options.RootComponentBehavior.IncludeSubcomponents))
            {
                var entity = entities.Single(e => e.MetadataId == component.ObjectId);
                if (entity.IsManaged.GetValueOrDefault())
                    failedEntities.Add(new ComponentInfo
                    {
                        ComponentId = component.ObjectId.GetValueOrDefault(), ComponentType = "Entity",
                        Hint = entity.LogicalName
                    });
            }

            if (failedEntities.Any())
                return new ConstraintCheckLogEntry
                {
                    ConstraintType = "Prevent ManagedEntitiesWithAllAssets",
                    Succeded = false,
                    ErrorComponents = failedEntities
                };

            return new ConstraintCheckLogEntry
            {
                ConstraintType = "Prevent ManagedEntitiesWithAllAssets"
            };
        }

        private ConstraintCheckLogEntry CheckForItemsWithouthActiveLayer(Guid originId)
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
                new ConditionExpression(SolutionComponent.LogicalNames.SolutionId, ConditionOperator.Equal, originId),
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

        private ConstraintCheckLogEntry CheckForFlows(Guid originId)
        {
            var components = GetSolutionFlowComponents(originId);

            if (components.Any())
                return new ConstraintCheckLogEntry
                {
                    ConstraintType = "Prevent Flows",
                    Succeded = false,
                    ErrorComponents = components.Select(c => new ComponentInfo
                        { ComponentId = c.ObjectId.GetValueOrDefault(), ComponentType = "Flow" }).ToList()
                };

            return new ConstraintCheckLogEntry
            {
                ConstraintType = "Prevent Flows",
                Succeded = true
            };
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
                var results = _executor.ElevatedOrganizationService.RetrieveMultiple(qe);
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

        private List<SolutionComponent> GetSolutionFlowComponents(Guid originId)
        {
            var components = new List<SolutionComponent>();
            var qe = new QueryExpression(SolutionComponent.EntityLogicalName)
            {
                NoLock = true,
                ColumnSet = new ColumnSet(true)
            };
            qe.Criteria.Conditions.AddRange(new List<ConditionExpression>
            {
                new ConditionExpression(SolutionComponent.LogicalNames.SolutionId, ConditionOperator.Equal, originId),
                new ConditionExpression(SolutionComponent.LogicalNames.ComponentType, ConditionOperator.Equal,
                    SolutionComponent.Options.ComponentType.Workflow)
            });

            var workflowLink = qe.AddLink(Workflow.EntityLogicalName, SolutionComponent.LogicalNames.ObjectId,
                Workflow.LogicalNames.WorkflowId);
            workflowLink.LinkCriteria.FilterOperator = LogicalOperator.Or;
            workflowLink.LinkCriteria.AddCondition(Workflow.LogicalNames.Category, ConditionOperator.Equal,
                Workflow.Options.Category.DesktopFlow);
            workflowLink.LinkCriteria.AddCondition(Workflow.LogicalNames.Category, ConditionOperator.Equal,
                Workflow.Options.Category.ModernFlow);

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
                var results = _executor.ElevatedOrganizationService.RetrieveMultiple(qe);
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
            var layers = _executor.ElevatedOrganizationService.RetrieveMultiple(query).Entities
                .Select(s => s.ToEntity<MsdynComponentlayer>()).ToList();
            return layers;
        }
    }
}