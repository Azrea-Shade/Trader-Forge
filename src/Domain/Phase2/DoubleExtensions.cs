using System;

namespace Domain
{
    public static class DoubleExtensions
    {
        public static bool HasValue(this double value) => !double.IsNaN(value);
    }
}
