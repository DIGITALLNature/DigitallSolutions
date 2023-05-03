using System;
using System.Collections.Generic;
using System.Linq;
using D365.Extension.Core;
using D365.Extension.Model;
using dgt.solutions.Plugins.Contract;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace dgt.solutions.Plugins.Helper
{
    internal class ComponentMover
    {
        private readonly Executor _executor;

        public ComponentMover(Executor executor)
        {
            _executor = executor;
        }

        internal IEnumerable<ComponentMoverLogEntry> MoveComponents(Guid originId, string destinationName)
        {
            return MoveComponents(GetSolutionComponents(new List<ConditionExpression>{
                new ConditionExpression(SolutionComponent.LogicalNames.SolutionId, ConditionOperator.Equal, originId)
            }), destinationName);
        }

        internal IEnumerable<ComponentMoverLogEntry> MoveAssemblyComponents(Guid originId, string destinationName)
        {
            var componentsTypes = new List<int>
            {
                SolutionComponent.Options.ComponentType.PluginAssembly,
                SolutionComponent.Options.ComponentType.SDKMessageProcessingStep
            };
            return MoveComponents(GetSolutionComponents(new List<ConditionExpression>{
                new ConditionExpression(SolutionComponent.LogicalNames.SolutionId, ConditionOperator.Equal, originId),
                new ConditionExpression(SolutionComponent.LogicalNames.ComponentType, ConditionOperator.In, componentsTypes.ToArray())
            }), destinationName);
        }

        private IEnumerable<SolutionComponent> GetSolutionComponents(IEnumerable<ConditionExpression> conditions)
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

        private IEnumerable<ComponentMoverLogEntry> MoveComponents(IEnumerable<SolutionComponent> components, string destinationName)
        {
            var log = new List<ComponentMoverLogEntry>();
            foreach (var component in components)
            {
                _executor.ElevatedOrganizationService.Execute(new AddSolutionComponentRequest
                {
                    AddRequiredComponents = false,
                    ComponentId = component.ObjectId.GetValueOrDefault(),
                    ComponentType = component.ComponentType.Value,
                    SolutionUniqueName = destinationName,
                    DoNotIncludeSubcomponents =
                        component.RootComponentBehavior?.Value == SolutionComponent.Options.RootComponentBehavior.DoNotIncludeSubcomponents ||
                        component.RootComponentBehavior?.Value == SolutionComponent.Options.RootComponentBehavior.IncludeAsShellOnly
                });
                log.Add(new ComponentMoverLogEntry
                {
                    ComponentId = component.ObjectId.GetValueOrDefault(),
                    ComponentType = component.FormattedValues.ContainsKey(SolutionComponent.LogicalNames.ComponentType) ? component.FormattedValues[SolutionComponent.LogicalNames.ComponentType] : $"ComponentType: {component.ComponentType}",
                    RootComponentBehavior = component.FormattedValues.ContainsKey(SolutionComponent.LogicalNames.RootComponentBehavior) ? component.FormattedValues[SolutionComponent.LogicalNames.RootComponentBehavior] : "not applicable"
                });
            }
            return log;
        }
    }
}
