using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParserCombinators;
using static ParserCombinators.BasicParsers;

namespace ParserTests
{
    [TestClass]
    public class ParserCore
    {
        [TestMethod]
        public void ShouldParseSameString()
        {
            var stringParser = BasicParsers.GetStringParser("whatever");
            var result = stringParser.Parse("whatever");
            Assert.AreNotEqual(typeof(Error<string>), result);
        }
        [TestMethod]
        public void ShouldFailToParseDifferentString()
        {
            var stringParser = BasicParsers.GetStringParser("whatever");
            var result = stringParser.Parse("hatever") as Error<string>;
            Assert.AreEqual("Expected \"whatever\", got \"hatever\".", result?.Message);
        }

        [TestMethod]
        public void ShouldReturnCorrectRestToParse()
        {
            var stringParser = BasicParsers.GetStringParser("whatever");
            var result = stringParser.Parse("whatever man");
            Assert.AreEqual(" man",result.Rest);
        }
        [TestMethod]
        public void ShouldParseRegularExpression()
        {
            var regexParser = GetRegexParser(new Regex("[a-z]+"));
            var result = regexParser.Parse("whatever multipe words");
            Assert.AreEqual("whatever", result.Output);
        }

        [TestMethod]
        public void ShouldFailToParseInvalidString()
        {
            var regexParser = GetRegexParser(new Regex("[a-z]+"));
            var result = regexParser.Parse("234345") as Error<string>;
            Assert.AreEqual("Expected match on '[a-z]+', got '234345'.", result?.Message);
        }

        [TestMethod]
        public void ShouldReturnCorrectRestToParse2()
        {
            var stringParser = GetRegexParser(new Regex("[a-z]+"));
            var result = stringParser.Parse("whatever man");
            Assert.AreEqual(" man", result.Rest);
        }

        [TestMethod]
        public void ShouldConcatenateParsers()
        {
            var identifier = R("[a-z]+");
            var functionParser = (identifier + S("(") + identifier + S(")")).Map(x =>
            {
                var identifiers = (IEnumerable<object>) x;
                Func<string> param = () => identifiers.Cast<string>().ElementAt(1);
                return param;
            });
            var result = functionParser.Parse("func(parameter)");
            Assert.AreNotEqual(typeof(Error<string>),result.GetType());
            Assert.AreEqual("parameter",((Func<string>)result.Output).Invoke());
        }

        [TestMethod]
        public void ShouldDiferentiateParsers()
        {
            var identifier = R("[a-z]+");
            var number = R("[0-9]+").Map(n=>int.Parse(n.ToString()));
            var parameter = identifier | number;
            var function = identifier + S("(") + parameter + S(")");
            var result = function.Parse("func(15)");
            var result2 = function.Parse("func(parameter)");
            Assert.AreNotEqual(typeof(Error<string>), result.GetType());
            Assert.AreNotEqual(typeof(Error<string>), result2.GetType());
        }

        [TestMethod]
        public void ShouldMatchOneOreMore2OfTheSameParser()
        {
            var number = R("[0-9]+").Name("number").Map(n => int.Parse(n.ToString()));
            var expression = (number+S("+").Name("plus").Optional()).OneOrMany().Name("expression");
            var result = expression.Parse("34 + 345 + 4");
            Assert.AreNotEqual(result.GetType(), typeof(Error<string>));
            Assert.AreEqual(3, (result.Output as IEnumerable<object>)?.Count());
        }


        [TestMethod]
        public void ShouldMatchOneOrMoreFails()
        {
            var number = R("[0-9]+").Map(n => int.Parse(n.ToString()));
            var expression = (number + S("+").Optional()).OneOrMany();
            var result = expression.Parse("fail");
            Assert.AreEqual( typeof(Error<string>), result.GetType());
        }

        [TestMethod]
        public void ShouldMatchOptionalParsers()
        {
            var identifier = R("[a-z]+");
            var number = R("[0-9]+").Map(n => int.Parse(n.ToString()));
            var functionCall = identifier + S("(") + number.Optional() + S(")");
            var result = functionCall.Parse("do()");
            var result2 = functionCall.Parse("do(45)");

            Assert.AreNotEqual(typeof(Error<string>),result.GetType());
            Assert.AreNotEqual(typeof(Error<string>),result2.GetType());
        }

        [TestMethod]
        public void ShouldNameParsers()
        {
            var identifier = R("[a-z]+");
            var namedIdentifier = identifier.Name("identifier");

            Assert.AreEqual("identifier", namedIdentifier.GetName());
        }

        [TestMethod]
        public void ShouldAllowMutuallyRecursiveParsers()
        {
            var identifier = R("[a-z,0-9]+").Name("identifier");
            var tag = (S("<") + identifier + S(">")).Name("tag");
            var tagEnd = (S("<\\") + identifier + S(">")).Name("tagEnd");
            var node=new Parser<string>().Name("node");
            var nodeList = node.OneOrMany().Optional().Name("nodeList");
            node.Func = (tag + nodeList + tagEnd).Name("node").Func;
            var result = node.Parse("<test><test2><\\test2><\\test>");

            Assert.AreNotEqual(typeof(Error<string>), result.GetType());
        }
    }
}
