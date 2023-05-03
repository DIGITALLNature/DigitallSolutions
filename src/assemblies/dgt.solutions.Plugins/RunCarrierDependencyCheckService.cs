using System;
using System.Linq;
using D365.Extension.Core;
using D365.Extension.Model;
using D365.Extension.Registration;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace dgt.solutions.Plugins
{
    [CustomApiRegistration(SdkMessageNames.DgtRunCarrierDependencyCheck)]
    public class RunCarrierDependencyCheckService : Executor
    {
        protected override ExecutionResult Execute()
        {
            var carrier = ElevatedOrganizationService.Retrieve(
                DgtCarrier.EntityLogicalName,
                Delegate.PluginExecutionContext.PrimaryEntityId, new ColumnSet(
                    DgtCarrier.LogicalNames.DgtSolutionuniquename,
                    DgtCarrier.LogicalNames.DgtSolutionversion
                )).ToEntity<DgtCarrier>();
            var dependencyCheckId = SecuredOrganizationService.Create(new DgtCarrierDependencyCheck
            {
                DgtCheckref = $"{carrier.DgtSolutionuniquename} - {carrier.DgtSolutionversion}",
                DgtCarrierId = carrier.ToEntityReference()
            });
            var request = new RetrieveMissingDependenciesRequest
            {
                SolutionUniqueName = carrier.DgtSolutionuniquename
            };
            var response = (RetrieveMissingDependenciesResponse)ElevatedOrganizationService.Execute(request);
            using (var context = DataContext(ElevatedOrganizationService))
            {
                foreach (var dependency in response.EntityCollection.Entities.Select(x => x.ToEntity<Dependency>()))
                {
                    if (dependency.RequiredComponentObjectId == null || Guid.Empty == dependency.RequiredComponentObjectId.Value) continue;

                    Delegate.TracingService.Trace($"{dependency.RequiredComponentObjectId}");
                    Delegate.TracingService.Trace($"{dependency.FormattedValues[Dependency.LogicalNames.RequiredComponentType]}");

                    var component = (from rec in context.SolutionComponentSet
                                     where rec.ObjectId != null
                                     where rec.ObjectId == dependency.RequiredComponentObjectId
                                     select rec).FirstOrDefault();

                    if (component == null) continue;

                    SecuredOrganizationService.Create(new DgtCarrierMissingDependency
                    {
                        DgtComponent = GetComponentName(context, component),
                        DgtCarrierDependencyCheckId = new EntityReference(DgtCarrierDependencyCheck.EntityLogicalName, dependencyCheckId),
                        DgtCarrierId = carrier.ToEntityReference(),
                        DgtRequiredComponentObjectid = dependency.RequiredComponentObjectId?.ToString("D"),
                        DgtRequiredComponentTypeName = dependency.FormattedValues[Dependency.LogicalNames.RequiredComponentType],
                        DgtRequiredComponentTypeNo = dependency.RequiredComponentType.Value
                    });
                }
            }

            return ExecutionResult.Ok;
        }

        private string GetComponentName(DataContext context, SolutionComponent component)
        {
            var name = $"{component.FormattedValues[SolutionComponent.LogicalNames.ComponentType]} - {{0}}";
            switch (component.ComponentType.Value)
            {
                // A separate method is required for each type of component
                case SolutionComponent.Options.ComponentType.Entity:
                    name = string.Format(name, GetEntityName(component.ObjectId.GetValueOrDefault()));
                    break;
                case SolutionComponent.Options.ComponentType.Attribute:
                    name = string.Format(name, GetAttributeName(component.ObjectId.GetValueOrDefault()));
                    break;
                case SolutionComponent.Options.ComponentType.OptionSet:
                    name = string.Format(name, GetOptionSetName(component.ObjectId.GetValueOrDefault()));
                    break;
                case SolutionComponent.Options.ComponentType.EntityRelationship:
                    name = string.Format(name, GetRelationshipName(component.ObjectId.GetValueOrDefault()));
                    break;
                case SolutionComponent.Options.ComponentType.WebResource:
                    name = string.Format(name, GetWebResourceName(context, component.ObjectId.GetValueOrDefault()));
                    break;
                case SolutionComponent.Options.ComponentType.SavedQuery:
                    name = string.Format(name, GetSavedQueryName(context, component.ObjectId.GetValueOrDefault()));
                    break;
                case SolutionComponent.Options.ComponentType.SystemForm:
                    name = string.Format(name, GetSystemFormName(context, component.ObjectId.GetValueOrDefault()));
                    break;
                case SolutionComponent.Options.ComponentType.PluginAssembly:
                    name = string.Format(name, GetPluginAssemblyName(context, component.ObjectId.GetValueOrDefault()));
                    break;
                case SolutionComponent.Options.ComponentType.PluginType:
                    name = string.Format(name, GetPluginTypeName(context, component.ObjectId.GetValueOrDefault()));
                    break;
                case SolutionComponent.Options.ComponentType.SDKMessage:
                    name = string.Format(name, GetSdkMessageName(context, component.ObjectId.GetValueOrDefault()));
                    break;
                case SolutionComponent.Options.ComponentType.SDKMessageProcessingStep:
                    name = string.Format(name, GetSdkMessageProcessingStepName(context, component.ObjectId.GetValueOrDefault()));
                    break;
                case SolutionComponent.Options.ComponentType.SDKMessageProcessingStepImage:
                    name = string.Format(name, GetSdkMessageProcessingStepImageName(context, component.ObjectId.GetValueOrDefault()));
                    break;
                default:
                    name = string.Format(name, component.ObjectId);
                    break;
            }
            return name;
        }

        private string GetAttributeName(Guid metadataId)
        {
            var request = new RetrieveAttributeRequest
            {
                MetadataId = metadataId
            };
            var response = (RetrieveAttributeResponse)ElevatedOrganizationService.Execute(request);
            return $"{response.AttributeMetadata.DisplayName.UserLocalizedLabel.Label} ({response.AttributeMetadata.SchemaName} / {response.AttributeMetadata.EntityLogicalName})";
        }

        private string GetOptionSetName(Guid metadataId)
        {
            var request = new RetrieveOptionSetRequest
            {
                MetadataId = metadataId
            };
            var response = (RetrieveOptionSetResponse)ElevatedOrganizationService.Execute(request);
            return response.OptionSetMetadata.DisplayName.UserLocalizedLabel.Label;
        }

        private string GetRelationshipName(Guid metadataId)
        {
            var request = new RetrieveRelationshipRequest
            {
                MetadataId = metadataId
            };
            var response = (RetrieveRelationshipResponse)ElevatedOrganizationService.Execute(request);
            return response.RelationshipMetadata.SchemaName;
        }

        private string GetEntityName(Guid metadataId)
        {
            var request = new RetrieveEntityRequest
            {
                MetadataId = metadataId
            };
            var response = (RetrieveEntityResponse)ElevatedOrganizationService.Execute(request);
            return $"{response.EntityMetadata.DisplayName.UserLocalizedLabel.Label} ({response.EntityMetadata.SchemaName})";
        }

        private string GetWebResourceName(DataContext context, Guid metadataId)
        {
            var webResource = (from rec in context.WebResourceSet
                               where rec.Id == metadataId
                               select rec).FirstOrDefault();
            return webResource?.Name;
        }

        private string GetSavedQueryName(DataContext context, Guid metadataId)
        {
            var savedQuery = (from rec in context.SavedQuerySet
                              where rec.Id == metadataId
                              select rec).FirstOrDefault();
            return savedQuery?.Name;
        }

        private string GetSystemFormName(DataContext context, Guid metadataId)
        {
            var systemForm = (from rec in context.SystemFormSet
                              where rec.Id == metadataId
                              select rec).FirstOrDefault();
            return systemForm?.Name;
        }

        private string GetPluginAssemblyName(DataContext context, Guid metadataId)
        {
            var pluginAssembly = (from rec in context.PluginAssemblySet
                                  where rec.Id == metadataId
                                  select rec).FirstOrDefault();
            return pluginAssembly?.Name;
        }

        private string GetPluginTypeName(DataContext context, Guid metadataId)
        {
            var pluginType = (from rec in context.PluginTypeSet
                              where rec.Id == metadataId
                              select rec).FirstOrDefault();
            return pluginType?.Name;
        }

        private string GetSdkMessageName(DataContext context, Guid metadataId)
        {
            var sdkMessage = (from rec in context.SdkMessageSet
                              where rec.Id == metadataId
                              select rec).FirstOrDefault();
            return sdkMessage?.Name;
        }

        private string GetSdkMessageProcessingStepName(DataContext context, Guid metadataId)
        {
            var sdkMessageProcessingStep = (from rec in context.SdkMessageProcessingStepSet
                                            where rec.Id == metadataId
                                            select rec).FirstOrDefault();
            return sdkMessageProcessingStep?.Name;
        }

        private string GetSdkMessageProcessingStepImageName(DataContext context, Guid metadataId)
        {
            var sdkMessageProcessingStepImage = (from rec in context.SdkMessageProcessingStepImageSet
                                                 join step in context.SdkMessageProcessingStepSet on rec.SdkMessageProcessingStepId.Id equals step.Id
                                                 where rec.Id == metadataId
                                                 select new { rec, step }).FirstOrDefault();
            return $"{sdkMessageProcessingStepImage?.rec.Name} ({sdkMessageProcessingStepImage?.step.Name})";
        }

        public override string GetConfig(string key, int lcid = 1033, string defaultValue = null)
        {
            return null;
        }
    }
}
