using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sprache;

namespace ParserTest
{
    class Program
    {
        static void SpracheParse()
        {
            StreamReader reader = new StreamReader("TestFile.xml");
            var parsed = XmlParser.Document.Parse(reader.ReadToEnd());
            reader.Close();
            Console.WriteLine(parsed);
            Console.ReadKey(true);
        }

        static void Main(string[] args)
        {
        }
    }
}
