using System.Threading.Tasks;

namespace Services
{
    public interface IEmailSender
    {
        Task SendAsync(string subject, string htmlBody, params string[] recipients);
    }

    public interface IToastNotifier
    {
        void ShowInfo(string title, string message);
    }
}
