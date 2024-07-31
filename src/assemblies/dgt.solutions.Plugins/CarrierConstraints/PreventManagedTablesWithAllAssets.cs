using System;
using System.Collections.Generic;
using System.Linq;
using D365.Extension.Core;
using D365.Extension.Model;
using D365.Extension.Registration;
using dgt.solutions.Plugins.Contract;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace dgt.solutions.Plugins.CarrierConstraints
{
    [CustomApiRegistration(SdkMessageNames.DgtPreventManagedTablesWithAllAssets)]
    public class PreventManagedTablesWithAllAssets : ConstraintBase
    {
        protected override string ConstraintType => "Prevent ManagedEntitiesWithAllAssets";

        protected override bool RunCheck(Guid solutionId)
        {
            var components = GetSolutionComponents(new List<ConditionExpression>
            {
                new ConditionExpression(SolutionComponent.LogicalNames.SolutionId, ConditionOperator.Equal, solutionId),
                new ConditionExpression(SolutionComponent.LogicalNames.ComponentType, ConditionOperator.Equal,
                    SolutionComponent.Options.ComponentType.Entity)
            });

            var request = new RetrieveAllEntitiesRequest
            {
                EntityFilters = EntityFilters.Entity
            };
            var response = (RetrieveAllEntitiesResponse)ElevatedOrganizationService.Execute(request);
            var entities = response.EntityMetadata.ToList();
            var failedEntities = new List<ComponentInfo>();

            foreach (var component in components.Where(c =>
                         c.RootComponentBehavior?.Value ==
                         SolutionComponent.Options.RootComponentBehavior.IncludeSubcomponents))
            {
                var entity = entities.Single(e => e.MetadataId == component.ObjectId);
                if (entity.IsManaged.GetValueOrDefault())
                {
                    WorkbenchHistoryLogger?.LogConstraintViolation(ConstraintType, "Entity", component.ObjectId.GetValueOrDefault(), $"Failed - Managed table contains all assets: {entity.LogicalName}");
                    failedEntities.Add(new ComponentInfo
                    {
                        ComponentId = component.ObjectId.GetValueOrDefault(),
                        ComponentType = "Entity",
                        Hint = entity.LogicalName
                    });
                }
                else
                {
                    WorkbenchHistoryLogger?.LogDebug($"Table: {entity.LogicalName}", ConstraintType, "Entity", component.ObjectId.GetValueOrDefault());
                }
            }

            if (failedEntities.Any()) return false;

            WorkbenchHistoryLogger?.LogConstraintSuccess(ConstraintType);
            return true;
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
    }
}