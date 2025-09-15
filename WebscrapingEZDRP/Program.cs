using System.Net;
using System.Net.Mail;
using System.Text;
using WebscrapingEZDRP;

class Program
{
    static async Task Main()
    {
        // Url stronki :)
        string url = "https://podrecznik.ezdrp.gov.pl/zmiany-w-wersjach/";

        // path do pliku z zapisanymi komunikatami
        string csvPath = @"EZDRP.csv";

        // path do ustawień maila
        string mailSettingsPath = @"mailsettings.txt";
        

        Html Html = new(url);
        CsvFile file = new(csvPath);

        // ostatniaAktualizacja - tekst na górze strony
        // listaZmian - wszystkie <li></li> na stronce

        (string ostatniaAktualizacja, List<string> listaZmian) = await Html.GetAsync();

        Console.WriteLine($"Znaleziono {listaZmian.Count} elementów <li> na stronie.");

        // zapisane w pliku <li>
        var seenLi = file.LoadSeenLi();

        // tylko nowe komunikaty
        var newRows = file.GetNewRows(ostatniaAktualizacja, listaZmian, seenLi);

        // jak cokolwiek nowego to dodawanie do pliku
        if (newRows.Any())
        {
            file.AppendRows(newRows);
            Console.WriteLine($"Dodano {newRows.Count} nowych rekordów.");

            // -------- wysyłka maila ---------
            try
            {
                // setx EZDRP_SMTP_USER "me@domain.com"
                // - tylko dla aktualnego użytkownika
                // setx EZDRP_SMTP_USER "me@domain.com" /M
                // - dla wszystkich adm
                var smtpUser = Environment.GetEnvironmentVariable("EZDRP_SMTP_USER");
                var smtpPass = Environment.GetEnvironmentVariable("EZDRP_SMTP_PASS");

                if (string.IsNullOrEmpty(smtpUser))
                    throw new InvalidOperationException("Brak adresu nadawcy (zmienna EZDRP_SMTP_USER nie jest ustawiona).");
                if (string.IsNullOrEmpty(smtpPass))
                    throw new InvalidOperationException("Brak hasła aplikacji (zmienna EZDRP_SMTP_PASS nie jest ustawiona).");

                // Ustawienia maila
                var cfg = MailSettings.Load(mailSettingsPath);

                var smtp = new SmtpClient("smtp.office365.com", 587)
                {
                    // emial, hasło aplikacji - https://mysignins.microsoft.com/security-info
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    EnableSsl = true
                };

                // -------- treść maila --------
                StringBuilder sb = new();

                sb.AppendLine("<h2>Informacja ze strony</h2>");
                sb.AppendLine("<p><a href=\"https://podrecznik.ezdrp.gov.pl/zmiany-w-wersjach/\">https://podrecznik.ezdrp.gov.pl/zmiany-w-wersjach/</a></p>");

                sb.AppendLine("<h2>Zmiany w wersjach</h2>");
                sb.AppendLine($"<p><b>{ostatniaAktualizacja}</b></p>");

                sb.AppendLine("<ul>");
                foreach (var row in newRows)
                {
                    sb.AppendLine(row.li);
                }
                sb.AppendLine("</ul>");

                var msg = cfg.CreateMessage(sb.ToString(), isHtml: true);

                smtp.Send(msg);
                Console.WriteLine("Wiadomość wysłana!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd: " + ex.Message);
            }

            // -------- wysyłka maila ---------
        }
        else
        {
            Console.WriteLine("Brak nowych rekordów.");
        }
    }
}
