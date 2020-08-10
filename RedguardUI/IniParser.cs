using System;
using System.Collections.Generic;
using System.IO;

namespace RedguardUI
{

    public class SimpleIniParser
    {
        public Dictionary<string, Dictionary<string, string>> Content { get; private set; }

        public bool StrictValue = true; //allows empty values when false
        public SimpleIniParser(string fileName, bool isStrict)
        {
            StrictValue = isStrict;
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

                else if (item.Contains("="))
                {
                    if (inSection)
                    {
                        string[] temp = item.Split('=');
                        if (StrictValue) if (temp[1] == string.Empty) throw new EmptyValueException(item);
                        if (temp[0] == string.Empty) throw new EmptyKeyException(item);
                        sections.Add(temp[0], temp[1]);
                    }
                    else throw new PairOutsideSectionException(item);
                }

                else if (item == string.Empty) { } //skip blank lines

                //if the item is anything else bail out hard
                else throw new InvalidDataException(item);
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
                output.Add(string.Empty); //blank line after each section to improve readability
            }
            File.WriteAllLines(fileName, output);
        }
    }

    internal class EmptyValueException : Exception { public EmptyValueException(string message) : base(message) { } }
    internal class EmptyKeyException : Exception { public EmptyKeyException(string message) : base(message) { } }
    internal class PairOutsideSectionException : Exception { public PairOutsideSectionException(string message) : base(message) { } }
}
