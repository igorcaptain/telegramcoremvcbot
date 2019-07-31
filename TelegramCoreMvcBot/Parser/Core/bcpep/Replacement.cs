using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.Core.bcpep
{
    class Replacement
    {
        public string Date { get; set; }
        public string Info { get; set; }
        public string Group { get; set; }
        public string Week { get; set; }
        public override string ToString()
        {
            return $"Заміни для {Group} на {Date}, {Week.ToLower()}:\n{Info}";
        }
        public Replacement(string group, string info, string date, string week)
        {
            Date = date; Info = info; Group = group; Week = week;
        }
        public Replacement() { }
    }
}
