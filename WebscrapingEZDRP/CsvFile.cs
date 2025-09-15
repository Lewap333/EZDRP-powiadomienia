using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;


namespace WebscrapingEZDRP
{
    internal class CsvFile
    {
        private readonly string _path;
        private readonly CsvConfiguration _config;

        public CsvFile(string path)
        {
            _path = path;
            _config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = true,
                IgnoreBlankLines = true,
                BadDataFound = null
            };
        }

        public HashSet<string> LoadSeenLi()
        {
            var seen = new HashSet<string>(StringComparer.Ordinal);
            if (!File.Exists(_path)) return seen;

            using var sr = new StreamReader(_path);
            using var csv = new CsvReader(sr, _config);
            foreach (var rec in csv.GetRecords<CsvRow>())
            {
                if (!string.IsNullOrEmpty(rec.li))
                    seen.Add(rec.li);
            }
            return seen;
        }

        public void AppendRows(List<CsvRow> rows)
        {
            bool needHeader = !File.Exists(_path);

            // Jak nie ma pliku to FileMode.Create, w innym przyppadku FileMode.Append

            using var fs = new FileStream(_path, needHeader ? FileMode.Create : FileMode.Append, FileAccess.Write, FileShare.Read);
            using var sw = new StreamWriter(fs);
            using var csvOut = new CsvWriter(sw, _config);

            if (needHeader)
            {
                csvOut.WriteHeader<CsvRow>();
                csvOut.NextRecord();
            }

            foreach (var row in rows)
            {
                csvOut.WriteRecord(row);
                csvOut.NextRecord();
            }
        }

        public List<CsvRow> GetNewRows(string entryMeta, List<string> liList, HashSet<string> seenLi)
        {
            var newRows = new List<CsvRow>();
            foreach (var li in liList)
            {
                if (!seenLi.Contains(li))
                    newRows.Add(new CsvRow { entry_meta = entryMeta, li = li });
            }
            return newRows;
        }
    }
}
