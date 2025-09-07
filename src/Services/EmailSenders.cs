using System;
using System.Linq;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using Serilog;

namespace Services
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly string? _apiKey;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public SendGridEmailSender()
        {
            _apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            _fromEmail = Environment.GetEnvironmentVariable("APP_SENT_FROM_EMAIL") ?? "no-reply@example.com";
            _fromName  = Environment.GetEnvironmentVariable("APP_SENT_FROM_NAME")  ?? "Azrea Companion";
        }

        public async Task SendAsync(string subject, string htmlBody, params string[] recipients)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                Log.Warning("Email disabled: missing SENDGRID_API_KEY. Subject='{Subject}' To={Count}", subject, recipients?.Length ?? 0);
                return; // No-op in CI or local without key
            }

            if (recipients == null || recipients.Length == 0) return;

            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(_fromEmail, _fromName);
            var tos  = recipients.Where(x => !string.IsNullOrWhiteSpace(x))
                                 .Distinct(StringComparer.OrdinalIgnoreCase)
                                 .Select(x => new EmailAddress(x.Trim()))
                                 .ToList();

            if (tos.Count == 0) return;

            var plain = StripTags(htmlBody);
            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, tos, subject, plain, htmlBody, false);
            var resp = await client.SendEmailAsync(msg);
            Log.Information("SendGrid status: {StatusCode}", resp.StatusCode);
        }

        private static string StripTags(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            var arr = new char[input.Length];
            int idx = 0;
            bool inside = false;
            foreach (var c in input)
            {
                if (c == '<') { inside = true; continue; }
                if (c == '>') { inside = false; continue; }
                if (!inside) arr[idx++] = c;
            }
            return new string(arr, 0, idx);
        }
    }
}
