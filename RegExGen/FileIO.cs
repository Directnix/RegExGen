﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RegExGen
{
    class FileIO
    {
        public static void saveFileString(string path, string fileContent)
        {
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.Write(fileContent);
                }
            }
            else
            {
                throw new SyntaxErrorException("FileIO error: file at: {" + path + "} already exists");
            }
        }

        public static string loadFileString(string path)
        {
            string output = "";
            if (File.Exists(path))
            {
                using (StreamReader sr = File.OpenText(path))
                {
                    string s;
                    while ((s = sr.ReadLine()) != null)
                    {
                        output += s;
                    }
                }
            }
            else
            {
                throw new SyntaxErrorException("FileIO error: file at: {" + path +"} doesn't exists");
            }

            return output;
        }

        public static void saveRegExListToTextFile(string path,List<RegExp> content)
        {
            foreach (RegExp c in content)
            {
                saveFileString(path, c.ToString());
            }
        }

        public static void saveRegExToTextFile(string path, RegExp content)
        {
            saveFileString(path, content.ToString());
        }

        public static List<RegExp> loadRegExesFromTextFile(string path)
        {
            List<RegExp> regExes = new List<RegExp>();

            string output = "";
            if (File.Exists(path))
            {
                using (StreamReader sr = File.OpenText(path))
                {
                    string s;
                    while ((s = sr.ReadLine()) != null)
                    {
                        regExes.Add(RegExParser.GetRegEx(s));
                    }
                }
            }
            else
            {
                throw new SyntaxErrorException("FileIO error: file at: {" + path + "} doesn't exists");
            }

            return regExes;
        }

        public static RegExp loadRegExFromTextFile(string path)
        {
            RegExp regEx = null;

            string output = "";
            if (File.Exists(path))
            {
                using (StreamReader sr = File.OpenText(path))
                {
                    string s;
                    while ((s = sr.ReadLine()) != null)
                    {
                        regEx = RegExParser.GetRegEx(s);
                    }
                }
            }
            else
            {
                throw new SyntaxErrorException("FileIO error: file at: {" + path + "} doesn't exists");
            }

            return regEx;

        }

        public static StringBuilder loadLanguage(string path)
        {
            StringBuilder output = new StringBuilder();
            if (File.Exists(path))
            {
                using (StreamReader sr = File.OpenText(path))
                {
                    string s;
                    while ((s = sr.ReadLine()) != null)
                    {
                        output.AppendLine(s);
                    }
                }
            }
            else
            {
                throw new SyntaxErrorException("FileIO error: file at: {" + path + "} doesn't exists");
            }

            return output;
        }

        public static void saveAutomataToTextFile(string path, Automata content)
        {
            string json = JsonConvert.SerializeObject(content);
            saveFileString(path, json);
        }

        public static Automata loadAutomataFromTextFile(string path)
        {
            return JsonConvert.DeserializeObject<Automata>(loadFileString(path));
        }
    }
}
