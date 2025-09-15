using System.Text.RegularExpressions;

namespace WebscrapingEZDRP
{
    internal class Html
    {
        private readonly string _url;
        public Html(string url) => _url = url;


        public async Task<(string entryMeta, List<string> liList)> GetAsync()
        {
            using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
            string html = await http.GetStringAsync(_url);

            // Regex na <div class="entry-meta">...</div> - ostatnia aktualizacja
            var metaMatch = Regex.Match(
                html,
                @"<div[^>]*\bclass\s*=\s*[""'][^""']*\bentry-meta\b[^""']*[""'][^>]*>(.*?)</div>",
                RegexOptions.IgnoreCase | RegexOptions.Singleline
            );

            string entryMeta = metaMatch.Success ? metaMatch.Groups[1].Value : "";

            // Regex na całe <ul></ul> w <div class="entry-content"> jako grupa 1.
            var ulInsideEntry = Regex.Match(
                html,
                @"<div[^>]*\bclass\s*=\s*[""'][^""']*\bentry-content\b[^""']*[""'][^>]*>.*?(<ul[^>]*\bclass\s*=\s*[""'][^""']*\bjo-art-list-ol\b[^""']*[""'][^>]*>.*?</ul>)",
                RegexOptions.IgnoreCase | RegexOptions.Singleline
            );

            // Regex na wszystkie <li>…</li>
            var liMatches = Regex.Matches(
                ulInsideEntry.Groups[1].Value,
                "<li\\b[^>]*>.*?</li>",
                RegexOptions.IgnoreCase | RegexOptions.Singleline
            );

            var liList = liMatches.Select(m => m.Value).ToList();

            return (entryMeta, liList);
        }
    }

}
