using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserCombinators;
using static ParserCombinators.BasicParsers;

namespace ParserTest
{
    public class PcXmlParser
    {
        public static Parser<string> GetXmlParser()
        {
            var comment = R("<!--.*?-->");
            var identifier = R("[a-zA-Z][a-zA-z09]+");
            var attribute = identifier + S("=\"") + identifier + S("\"");
            var tag = S("<") + identifier + attribute.OneOrMany(" ").Optional() + S(">");
            var endTag = S("<\\") + identifier + S(">");
            Parser<string> node;
            var nodeList = node.OneOrMany("\r\n");
            node = tag+nodeList+endTag;
        }
    }
}
