using System;
using R4RAPI.Models;

namespace R4RAPI.Services
{
    /// <summary>
    /// Interface for any concrete implementation of Resource Aggregation Services
    /// </summary>
    public interface IResourceAggregationService
    {
        /// <summary>
        /// Gets the key label aggregation for a field
        /// </summary>
        /// <returns>The key label aggregation.</returns>
        /// <param name="field">The field to aggregate</param>
        /// <param name="query">The query for the results</param>
        KeyLabelAggResult[] GetKeyLabelAggregation(string field, ResourceQuery query);
    }
}
