using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Domain;
using Services;

namespace Presentation
{
    public static class ServiceAdapters
    {
        private static object? GetRepo(PortfolioService svc)
        {
            var t = svc.GetType();
            var f = t.GetField("_repo", BindingFlags.Instance|BindingFlags.NonPublic)
                 ?? t.GetField("_portfolios", BindingFlags.Instance|BindingFlags.NonPublic);
            return f?.GetValue(svc);
        }

        public static IEnumerable<Portfolio> AllPortfolios(this PortfolioService svc)
        {
            var m = svc.GetType().GetMethod("AllPortfolios", BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic, new Type[0]);
            if (m != null && m.ReturnType != typeof(void))
            {
                var r = m.Invoke(svc, null);
                if (r is IEnumerable<Portfolio> typed) return typed;
                if (r is IEnumerable<object> objs) return objs.OfType<Portfolio>();
            }

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

        public static PortfolioSummary BuildSummaryTyped(this PortfolioService svc, int portfolioId)
        {
            var m = svc.GetType().GetMethod("BuildSummary", BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic, new Type[] { typeof(int) });
            if (m != null)
            {
                var r = m.Invoke(svc, new object[] { portfolioId });
                if (r is PortfolioSummary ps) return ps;
                if (r is object o)
                {
                    var ps2 = new PortfolioSummary();
                    var allocProp = o.GetType().GetProperty("Allocation");
                    if (allocProp != null)
                    {
                        var v = allocProp.GetValue(o);
                        if (v is IDictionary<string, decimal> d) ps2.Allocation = new Dictionary<string, decimal>(d);
                        else if (v is IDictionary<string, double> d2)
                        {
                            var map = new Dictionary<string, decimal>();
                            foreach (var kv in d2) map[kv.Key] = (decimal)kv.Value;
                            ps2.Allocation = map;
                        }
                    }
                    return ps2;
                }
            }

            dynamic repo = GetRepo(svc) ?? throw new InvalidOperationException("Portfolio repository not available.");
            var res = repo.BuildSummary(portfolioId);
            return res is PortfolioSummary ok ? ok : new PortfolioSummary();
        }
    }
}
