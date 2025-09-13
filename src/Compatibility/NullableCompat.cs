namespace Compatibility
{
    public static class NullableCompat
    {
        public static bool HasValue(this double _) => true; // safety shim; real code uses double?
    }
}
