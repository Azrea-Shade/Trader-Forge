using System;
using System.Collections.Generic;
using System.Linq;
using Domain;

namespace Presentation
{
    public static class ServiceAdapters
    {
        /// <summary>
        /// Convert allocation weights (ticker->weight) into a strongly-typed PortfolioSummary.
        /// </summary>
        public static PortfolioSummary ToPortfolioSummary(this IDictionary<string, decimal> weights)
        {
            var rows = weights?
                .Select(kv => new AllocationRow(kv.Key, kv.Value))
                .ToList() ?? new List<AllocationRow>();
            return new PortfolioSummary(rows);
        }

        /// <summary>
        /// Best-effort conversion from anything the service returns:
        /// - PortfolioSummary (pass-through)
        /// - Dictionary&lt;string, decimal&gt; (map to rows)
        /// - null or unexpected: empty summary
        /// </summary>
        public static PortfolioSummary ToPortfolioSummaryLoose(this object? any)
        {
            if (any is PortfolioSummary ps) return ps;

            if (any is IDictionary<string, decimal> dict) return dict.ToPortfolioSummary();

            // Try dynamic dictionary from Dapper (e.g., Dictionary<string, object> with decimals)
            if (any is System.Collections.IDictionary genericDict)
            {
                var rows = new List<AllocationRow>();
                foreach (System.Collections.DictionaryEntry de in genericDict)
                {
                    if (de.Key is string ticker)
                    {
                        // Try to coerce value to decimal
                        try
                        {
                            var weight = Convert.ToDecimal(de.Value);
                            rows.Add(new AllocationRow(ticker, weight));
                        }
                        catch { /* skip bad rows */ }
                    }
                }
                return new PortfolioSummary(rows);
            }

            // Nothing usable - return empty
            return new PortfolioSummary(new List<AllocationRow>());
        }
    }
}
