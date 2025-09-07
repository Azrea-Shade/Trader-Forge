namespace Services
{
    /// <summary>Minimal notification contract used by Scheduler and tests.</summary>
    public interface INotifier
    {
        void Notify(string title, string message);
    }
}
