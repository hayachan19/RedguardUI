using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace RedguardUI
{
    class MilesParser
    {
        public Dictionary<string, string> Content { get; private set; }

        public MilesParser(string fileName)
        {
            Read(fileName);
        }

        private void Read(string fileName)
        {
            Content = new Dictionary<string, string>();
            string[] file = File.ReadAllLines(fileName);

            foreach (string item in file)
            {
                if (!item.StartsWith(";") && !string.IsNullOrWhiteSpace(item))
                {
                    string split = Regex.Replace(item, @"\s{2,}", "\t");
                    string[] temp = split.Split('\t');
                    Content.Add(temp[0], temp[1]);
                }
            }
        }
    }
}
