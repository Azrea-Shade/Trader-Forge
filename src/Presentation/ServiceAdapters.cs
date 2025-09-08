using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Domain;

namespace Presentation
{
    public static class ServiceAdapters
    {
        // --- helpers ---
        private static object? GetRepo(object svc)
        {
            var t = svc.GetType();
            var f = t.GetField("_repo", BindingFlags.Instance|BindingFlags.NonPublic)
                 ?? t.GetField("_portfolios", BindingFlags.Instance|BindingFlags.NonPublic);
            return f?.GetValue(svc);
        }

        private static List<AllocationRow> MapAllocation(object? value)
        {
            var list = new List<AllocationRow>();
            if (value is IEnumerable<AllocationRow> rows)
                return rows.ToList();

            if (value is IDictionary<string, decimal> mapD)
            {
                foreach (var kv in mapD)
                {
                    var row = new AllocationRow();
                    var rt = row.GetType();
                    (rt.GetProperty("Symbol") ?? rt.GetProperty("Ticker") ?? rt.GetProperty("Name"))?.SetValue(row, kv.Key);
                    var wp = rt.GetProperty("Weight") ?? rt.GetProperty("Percent") ?? rt.GetProperty("Allocation");
                    if (wp != null)
                    {
                        var tp = Nullable.GetUnderlyingType(wp.PropertyType) ?? wp.PropertyType;
                        wp.SetValue(row, Convert.ChangeType(kv.Value, tp));
                    }
                    list.Add(row);
                }
                return list;
            }

            if (value is IDictionary<string, double> mapF)
            {
                foreach (var kv in mapF)
                {
                    var row = new AllocationRow();
                    var rt = row.GetType();
                    (rt.GetProperty("Symbol") ?? rt.GetProperty("Ticker") ?? rt.GetProperty("Name"))?.SetValue(row, kv.Key);
                    var wp = rt.GetProperty("Weight") ?? rt.GetProperty("Percent") ?? rt.GetProperty("Allocation");
                    if (wp != null)
                    {
                        var tp = Nullable.GetUnderlyingType(wp.PropertyType) ?? wp.PropertyType;
                        wp.SetValue(row, Convert.ChangeType((decimal)kv.Value, tp));
                    }
                    list.Add(row);
                }
                return list;
            }

            return list; // empty if nothing recognized
        }

        // --- public adapters (typed) ---
        public static IEnumerable<Portfolio> AllPortfolios(this Services.PortfolioService svc)
        {
            // prefer any native typed method if present
            var m = svc.GetType().GetMethod("AllPortfolios", BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic, new Type[0]);
            if (m != null && m.ReturnType != typeof(void))
            {
                var r = m.Invoke(svc, null);
                if (r is IEnumerable<Portfolio> typed) return typed;
                if (r is IEnumerable<object> objs) return objs.OfType<Portfolio>();
            }

            // fallback to repository dynamic rows -> typed
            dynamic repo = GetRepo(svc) ?? throw new InvalidOperationException("Portfolio repository not available.");
            IEnumerable<dynamic> rows = repo.AllPortfolios();
            var list = new List<Portfolio>();
            foreach (var row in rows)
            {
                if (row is Portfolio p) { list.Add(p); continue; }
                var t = row.GetType();
                int id = Convert.ToInt32(t.GetProperty("Id")?.GetValue(row) ?? t.GetProperty("id")?.GetValue(row) ?? 0);
                string name  = Convert.ToString(t.GetProperty("Name")?.GetValue(row) ?? t.GetProperty("name")?.GetValue(row) ?? "") ?? "";
                string notes = Convert.ToString(t.GetProperty("Notes")?.GetValue(row)?? t.GetProperty("notes")?.GetValue(row)?? "") ?? "";
                list.Add(new Portfolio { Id = id, Name = name, Notes = notes });
            }
            return list;
        }

        public static PortfolioSummary BuildSummaryTyped(this Services.PortfolioService svc, int portfolioId)
        {
            // if the service already has a strongly-typed method, prefer it
            var m = svc.GetType().GetMethod("BuildSummary", BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic, new Type[] { typeof(int) });
            if (m != null && m.ReturnType != typeof(void))
            {
                var r = m.Invoke(svc, new object[] { portfolioId });
                if (r is PortfolioSummary ps) return ps;
            }

            // fallback through the repo
            dynamic repo = GetRepo(svc) ?? throw new InvalidOperationException("Portfolio repository not available.");
            var res = repo.BuildSummary(portfolioId);
            if (res is PortfolioSummary ok) return ok;

            var ps2 = new PortfolioSummary();
            if (res != null)
            {
                var rt = res.GetType();
                var allocProp = rt.GetProperty("Allocation") ?? rt.GetProperty("Allocations");
                if (allocProp != null)
                {
                    var val = allocProp.GetValue(res);
                    ps2.Allocation = MapAllocation(val);
                }
            }
            return ps2;
        }
    }
}
