using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shogi.Data
{
    public class RecordEntry
    {
        public string Name { get; }
        public int Score { get; }

        public RecordEntry(string name, int score)
        {
            Name = name;
            Score = score;
        }

        public override string ToString() => $"{Name};{Score}";

        public static RecordEntry FromString(string line)
        {
            var parts = line.Split(';');
            return new RecordEntry(parts[0], int.Parse(parts[1]));
        }
    }
}
