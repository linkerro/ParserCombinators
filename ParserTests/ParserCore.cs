using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParserCombinators;

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
            Assert.AreEqual("whatever", result.Output);
        }
        [TestMethod]
        public void ShouldFailToParseDifferentString()
        {
            var stringParser = BasicParsers.GetStringParser("whatever");
            var result = stringParser.Parse("hatever") as Error<string,string>;
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
            var regexParser = BasicParsers.GetRegexParser(new Regex("[a-z]+"));
            var result = regexParser.Parse("whatever multipe words");
            Assert.AreEqual("whatever", result.Output);
        }

        [TestMethod]
        public void ShouldFailToParseInvalidString()
        {
            var regexParser = BasicParsers.GetRegexParser(new Regex("[a-z]+"));
            var result = regexParser.Parse("234345") as Error<string,string>;
            Assert.AreEqual("Expected match on '[a-z]+', got '234345'.", result?.Message);
        }

        [TestMethod]
        public void ShouldReturnCorrectRestToParse2()
        {
            var stringParser = BasicParsers.GetRegexParser(new Regex("[a-z]+"));
            var result = stringParser.Parse("whatever man");
            Assert.AreEqual(" man", result.Rest);
        }
    }
}
