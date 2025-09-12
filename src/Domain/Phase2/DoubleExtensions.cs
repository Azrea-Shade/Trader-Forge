using System;

namespace Domain
{
    public static class DoubleExtensions
    {
        /// <summary>
        /// Returns true when the double is a real number (not NaN).
        /// </summary>
        public static bool HasValue(this double value) => !double.IsNaN(value);
    }
}
