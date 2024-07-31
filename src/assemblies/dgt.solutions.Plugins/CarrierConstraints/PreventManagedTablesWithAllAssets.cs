using System;
using System.Collections.Generic;
using System.Linq;
using D365.Extension.Core;
using D365.Extension.Model;
using D365.Extension.Registration;
using dgt.solutions.Plugins.Helper;
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
            var passed = true;

            foreach (var component in components.Where(c =>
                         c.RootComponentBehavior?.Value ==
                         SolutionComponent.Options.RootComponentBehavior.IncludeSubcomponents))
            {
                var entity = entities.Single(e => e.MetadataId == component.ObjectId);
                if (entity.IsManaged.GetValueOrDefault())
                {
                    WorkbenchHistoryLogger?.LogConstraintViolation(ConstraintType, "Entity", component.ObjectId.GetValueOrDefault(), $"Failed - Managed table contains all assets: {entity.LogicalName}");
                    passed = false;
                }
                else
                {
                    WorkbenchHistoryLogger?.LogDebug($"Table: {entity.LogicalName}", ConstraintType, "Entity", component.ObjectId.GetValueOrDefault());
                }
            }

            if (!passed) return false;

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
    }
}
