using System.Net.Mail;

namespace WebscrapingEZDRP
{
    internal class MailSettings
    {
        private string? From = Environment.GetEnvironmentVariable("EZDRP_SMTP_USER");

        public List<string> To { get; set; } = new();
        public List<string> Cc { get; set; } = new();   // DW
        public List<string> Bcc { get; set; } = new();  // UDW

        public string? Subject { get; set; }

        public static MailSettings Load(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Brak pliku ustawień maila: {path}");

            var cfg = new MailSettings();

            foreach (var raw in File.ReadAllLines(path))
            {
                var line = raw.Trim();
                if (string.IsNullOrEmpty(line) || line.StartsWith("#")) continue;

                var parts = line.Split('=', 2);
                if (parts.Length != 2) continue;

                var key = parts[0].Trim().ToLowerInvariant();
                var value = parts[1].Trim();

                switch (key)
                {
                    case "to":
                        cfg.To = SplitEmails(value);
                        break;
                    case "cc":
                        cfg.Cc = SplitEmails(value);
                        break;
                    case "bcc":
                        cfg.Bcc = SplitEmails(value);
                        break;
                    case "subject":
                        cfg.Subject = value;
                        break;
                }
            }

            return cfg;
        }

        private static List<string> SplitEmails(string csv)
        {
            return csv.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                      .Select(x => x.Trim())
                      .Where(x => !string.IsNullOrEmpty(x))
                      .ToList();
        }

        public MailMessage CreateMessage(string body, bool isHtml = true)
        {
            var msg = new MailMessage();

            if (string.IsNullOrEmpty(From))
                throw new InvalidOperationException("Brak adresu nadawcy (zmienna EZDRP_SMTP_USER nie jest ustawiona).");

            msg.From = new MailAddress(From);

            foreach (var r in To) msg.To.Add(r);
            foreach (var r in Cc) msg.CC.Add(r);
            foreach (var r in Bcc) msg.Bcc.Add(r);

            msg.Subject = Subject ?? "(brak tematu)";
            msg.Body = body;
            msg.IsBodyHtml = isHtml;

            return msg;
        }
    }
}
