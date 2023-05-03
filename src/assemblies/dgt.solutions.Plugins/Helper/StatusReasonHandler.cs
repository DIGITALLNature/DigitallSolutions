using System;
using D365.Extension.Core;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;

namespace dgt.solutions.Plugins.Helper
{
    internal class StatusReasonHandler
    {
        private readonly Executor _executor;

        public StatusReasonHandler(Executor executor)
        {
            _executor = executor;
        }

        internal void Update(Entity update, Entity create = null)
        {
            //new trx needed to skip the rollback
            var executeMultipleRequest = new ExecuteMultipleRequest
            {
                Settings = new ExecuteMultipleSettings
                {
                    ContinueOnError = true,
                    ReturnResponses = false
                },
                Requests = new OrganizationRequestCollection()
            };
            executeMultipleRequest.Requests.Add(new UpdateRequest
            {
                Target = update
            });
            if (create != null)
            {
                executeMultipleRequest.Requests.Add(new CreateRequest
                {
                    Target = create
                });
            }
            try
            {
                _executor.SecuredOrganizationService.Execute(executeMultipleRequest);
            }
            catch (Exception e)
            {
                _executor.LoggingService.Error(e.RootMessage(), e);
                throw new InvalidPluginExecutionException(e.RootMessage());//here the logic is broken
            }
        }
    }
}
