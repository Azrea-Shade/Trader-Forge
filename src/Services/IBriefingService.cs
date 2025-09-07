using System;
using Domain;

namespace Services
{
    public interface IBriefingService
    {
        DailyBrief Generate(DateOnly date, string[] tickers);
    }
}
