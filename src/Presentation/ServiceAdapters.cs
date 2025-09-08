using System;
using System.Collections.Generic;
using System.Linq;
using Domain;

namespace Presentation
{
    public static class ServiceAdapters
    {
        // Convert a dictionary (ticker -> weight) into a list of AllocationRow
        public static List<AllocationRow> MapAllocation(IDictionary<string, decimal> map)
        {
            if (map == null) return new List<AllocationRow>();
            return map.Select(kv => new AllocationRow(kv.Key, kv.Value)).ToList();
        }

        // Build a typed PortfolioSummary when you have id/name/allocation map
        public static PortfolioSummary BuildSummaryTyped(int id, string name, IDictionary<string, decimal> allocationMap)
        {
            var allocation = MapAllocation(allocationMap);
            return new PortfolioSummary(id, name, allocation);
        }

        /// <summary>
        /// Convert allocation weights (ticker->weight) into a strongly-typed PortfolioSummary.
        /// Uses an empty id/name when only allocation rows are available.
        /// </summary>
        public static PortfolioSummary ToPortfolioSummary(this IDictionary<string, decimal> weights)
        {
            var rows = weights?
                .Select(kv => new AllocationRow(kv.Key, kv.Value))
                .ToList() ?? new List<AllocationRow>();

            // use default id/name when only allocations are present
            return new PortfolioSummary(0, string.Empty, rows);
        }

        /// <summary>
        /// Best-effort conversion from anything the service returns:
        /// - PortfolioSummary (pass-through)
        /// - Dictionary&lt;string, decimal&gt; (map to rows)
        /// - System.Collections.IDictionary (Dapper dynamic rows) -> coerce to decimals where possible
        /// - otherwise: empty summary
        /// </summary>
        public static PortfolioSummary ToPortfolioSummaryLoose(this object? any)
        {
            if (any is PortfolioSummary ps) return ps;
            if (any is IDictionary<string, decimal> dict) return dict.ToPortfolioSummary();

            if (any is System.Collections.IDictionary genericDict)
            {
                var rows = new List<AllocationRow>();
                foreach (System.Collections.DictionaryEntry de in genericDict)
                {
                    if (de.Key is string ticker)
                    {
                        try
                        {
                            var weight = Convert.ToDecimal(de.Value);
                            rows.Add(new AllocationRow(ticker, weight));
                        }
                        catch
                        {
                            // skip rows we can't convert
                        }
                    }
                }
                return new PortfolioSummary(0, string.Empty, rows);
            }

            return new PortfolioSummary(0, string.Empty, new List<AllocationRow>());
        }
    }
}
