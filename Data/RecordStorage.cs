using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shogi.Data
{
    public class RecordStorage
    {
        private readonly string _path;

        public RecordStorage(string path)
        {
            _path = path;
        }

        public List<RecordEntry> LoadRecords()
        {
            if (!File.Exists(_path))
                return new List<RecordEntry>();

            return File.ReadAllLines(_path)
                       .Select(RecordEntry.FromString)
                       .OrderByDescending(r => r.Score)
                       .ToList();
        }

        public void AddRecord(RecordEntry entry)
        {
            File.AppendAllText(_path, entry + Environment.NewLine);
        }

    }

}
