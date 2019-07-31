using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;

namespace Parser.Core.bcpep
{
    class BcpepParser : IParser<Replacement[]>
    {
        public Replacement[] Parser(IHtmlDocument document)
        {
            List<List<string>> allItems = new List<List<string>>();
            if (document.BaseUri.Length < 20) return null;
            string date = document.QuerySelector("div h3 span span").TextContent;
            string group = "";
            string week = document.QuerySelector("h3 + h3 + h3 span span").TextContent;
            var items = document.QuerySelectorAll("table tr");
            foreach(var item in items)
            {
                var cells = item.QuerySelectorAll("td");
                List<string> row = new List<string>();
                foreach (var cell in cells)
                    row.Add(cell.TextContent.Replace("\n", ""));
                if (row.Count < 5)
                    row.Insert(0, "");
                if (row[0].Length < 4)
                    row[0] = "";
                row[1] = row[1].Replace(" – ", "").Trim();
                row[3] = (row[3].Length > 4 && row[4].Length > 3) ? $" {row[3]} {row[4]}" : "";
                row.Remove(row[4]);
                allItems.Add(row);
            }
            List<Replacement> replacemetns = new List<Replacement>();
            foreach(var item in allItems)
            {
                if(item[0].Contains("-"))
                {
                    int index = allItems.IndexOf(item);
                    group = item[0];
                    string groupinfo = "";
                    do
                    {
                        groupinfo += $" {allItems[index][1]} {allItems[index][2]}{allItems[index++][3]};\n";
                    }
                    while (index < allItems.Count && allItems[index][0] == "");
                    replacemetns.Add(new Replacement(group, groupinfo, date, week));
                }
            }
            return replacemetns.ToArray();
        }
    }
}
