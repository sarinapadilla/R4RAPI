using System;
using System.Threading.Tasks;
using NCI.OCPL.Api.ResourcesForResearchers.Models;

namespace NCI.OCPL.Api.ResourcesForResearchers.Services
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

        /// <summary>
        /// Asynchronously gets the key label aggregation for a field
        /// </summary>
        /// <param name="field">The field to aggregate</param>
        /// <param name="query">The query for the results</param>
        /// <returns>The aggregation items</returns>
        Task<KeyLabelAggResult[]> GetKeyLabelAggregationAsync(string field, ResourceQuery query);
    }
}
