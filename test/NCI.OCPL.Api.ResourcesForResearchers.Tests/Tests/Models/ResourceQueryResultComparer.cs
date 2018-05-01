using System;
using System.Collections.Generic;
using System.Text;
using NCI.OCPL.Api.ResourcesForResearchers.Models;

namespace NCI.OCPL.Api.ResourcesForResearchers.Tests.Models
{
    public class ResourceQueryResultComparer : IEqualityComparer<ResourceQueryResult>
    {
        public bool Equals(ResourceQueryResult x, ResourceQueryResult y)
        {
            if (x == y)
                return true;

            if (x == null || y == null)
                return false;

            return false;
        }

        public int GetHashCode(ResourceQueryResult obj)
        {
            throw new NotImplementedException();
        }
    }
}
