using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace dgt.solutions.Plugins.Helper
{
    public static class PagingOrganizationServiceExtension
    {
        public static IEnumerable<TEntity> RetrieveMultiplePaged<TEntity>(this IOrganizationService service, QueryExpression query, ITracingService tracingService = default) where TEntity : Entity
        {
            EntityCollection results;
            query.PageInfo = new PagingInfo
            {
                Count = 5000,
                PageNumber = 1,
                PagingCookie = null
            };
            do
            {
                tracingService?.Trace($"Retrieving page {query.PageInfo.PageNumber}");
                results = service.RetrieveMultiple(query);

                foreach (var entity in results.Entities)
                {
                    yield return entity.ToEntity<TEntity>();
                }

                query.PageInfo.PageNumber++;
                query.PageInfo.PagingCookie = results.PagingCookie;
            } while (results.MoreRecords);
        }
    }
}
