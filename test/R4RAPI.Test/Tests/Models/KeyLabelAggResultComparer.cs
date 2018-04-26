using System;
using System.Collections.Generic;

using R4RAPI.Models;

namespace R4RAPI.Test.Models
{
    /// <summary>
    /// Key label agg result comparer.
    /// </summary>
    public class KeyLabelAggResultComparer : IEqualityComparer<KeyLabelAggResult>
    {
        public KeyLabelAggResultComparer()
        {
        }

        /// <summary>
        /// Determines if two KeyLabelAggResult objects are equivelent
        /// </summary>
        /// <returns>True if the two objects are equivelent</returns>
        /// <param name="x">The first KeyLabelAggResult</param>
        /// <param name="y">The second KeyLabelAggResult</param>
        public bool Equals(KeyLabelAggResult x, KeyLabelAggResult y)
        {
            if (x == y)
                return true;

            if (x == null || y == null)
                return false;

            return x.Count == y.Count &&
                    x.Key == y.Key &&
                    x.Label == y.Label;
        }

        public int GetHashCode(KeyLabelAggResult obj)
        {
            throw new NotImplementedException();
        }
    }
}
