using System;
using System.Collections.Generic;
using System.IO;

namespace RedguardUI
{
    public class SimpleIniParser
    {
        public Dictionary<string, Dictionary<string, string>> Content { get; private set; }

        public SimpleIniParser(string fileName)
        {
            Read(fileName);
        }

        private void Read(string fileName)
        {
            Content = new Dictionary<string, Dictionary<string, string>>();
            bool inSection = false;
            string currentSection = null;
            string[] file = File.ReadAllLines(fileName);
            Dictionary<string, string> sections = null;

            foreach (string item in file)
            {
                if (item.StartsWith("[") && item.EndsWith("]"))
                {
                    if (inSection)
                    {
                        Content.Add(currentSection, sections);
                        inSection = false;
                    }
                    sections = new Dictionary<string, string>();
                    currentSection = item.Trim(new char[] { '[', ']' });
                    inSection = true;
                }
                if (item.Contains("="))
                {
                    if (inSection)
                    {
                        string[] temp = item.Split('=');
                        sections.Add(temp[0], temp[1]);
                    }
                    else throw new Exception("Key pair outside section");
                }
            }
            Content.Add(currentSection, sections); //add last section
        }

        public void Save(string fileName)
        { 
            List<string> output = new List<string>();
            foreach (var section in Content)
            {
                output.Add("[" + section.Key + "]");
                foreach (var setting in section.Value)
                {
                    output.Add(setting.Key + "=" + setting.Value);
                }
            }
            File.WriteAllLines(fileName, output);
        }
    }
}
