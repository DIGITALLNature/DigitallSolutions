using System;
using System.Collections.Generic;
using System.Linq;
using D365.Extension.Core;
using D365.Extension.Model;
using Microsoft.Xrm.Sdk;

namespace dgt.solutions.Plugins.Helper
{
    internal class SolutionLookup
    {
        private readonly Executor _executor;

        public SolutionLookup(Executor executor)
        {
            _executor = executor;
        }

        internal Solution GetByName(string uniqueName)
        {
            using (var dataContext = _executor.DataContext(_executor.ElevatedOrganizationService))
            {
                var solution = (from rec in dataContext.SolutionSet
                                where rec.UniqueName == uniqueName
                                select rec).SingleOrDefault();
                if (solution == null) throw new InvalidPluginExecutionException($"Solution with name {uniqueName} not found!");
                return solution;
            }
        }

        internal List<Solution> GetPatchesByParent(Guid parentId)
        {
            using (var dataContext = _executor.DataContext(_executor.ElevatedOrganizationService))
            {
                return (from rec in dataContext.SolutionSet
                        where rec.ParentSolutionId != null
                        where rec.ParentSolutionId.Id == parentId
                        //orderby rec.CreatedOn descending
                        select rec).ToList();
            }
        }
    }
}
