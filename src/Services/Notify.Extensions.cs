namespace Services
{
    public static class NotifierExtensions
    {
        // If interface only exposes Notify(level,msg), provide Info/Warn/Error helpers.
        public static void Info(this INotifier n, string message)  { n.Notify("INFO",  message); }
        public static void Warn(this INotifier n, string message)  { n.Notify("WARN",  message); }
        public static void Error(this INotifier n, string message) { n.Notify("ERROR", message); }
    }
}
