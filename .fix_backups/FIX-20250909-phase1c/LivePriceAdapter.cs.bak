using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using Services.Feeds;

namespace Services
{
    /// <summary>
    /// Lightweight LivePriceAdapter placeholder that provides a typed helper to build PortfolioSummary.
    /// This keeps the merge/resolution simple; replace/extend logic later if you need live feed behaviour.
    /// </summary>
    public class LivePriceAdapter
    {
        public LivePriceAdapter() { }

        /// <summary>
        /// Build a PortfolioSummary from an allocation map (ticker -> weight).
        /// Use id/name when available.
        /// </summary>
        public PortfolioSummary BuildSummaryTyped(int id = 0, string? name = null, IDictionary<string, decimal>? allocationMap = null)
        {
            var rows = allocationMap?
                .Select(kv => new AllocationRow(kv.Key, kv.Value))
                .ToList() ?? new List<AllocationRow>();

            return new PortfolioSummary(id, name ?? string.Empty, rows);
        }

        // A permissive converter that accepts a dynamic-ish object and tries to produce a PortfolioSummary.
        // This mirrors the "loose" adapter idea used in Presentation so callers that pass dictionaries or typed
        // Domain objects will still get a PortfolioSummary back.
        public PortfolioSummary BuildSummaryLoose(object? any)
        {
            if (any is PortfolioSummary ps) return ps;
            if (any is IDictionary<string, decimal> dict) return BuildSummaryTyped(0, string.Empty, dict);
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
                        catch { /* skip unconvertible */ }
                    }
                }
                return new PortfolioSummary(0, string.Empty, rows);
            }

            return new PortfolioSummary(0, string.Empty, new List<AllocationRow>());
        }
    }
}
