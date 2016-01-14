using System;
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
            Assert.AreEqual("whatever", result);
        }
        [TestMethod]
        public void ShouldFailToParseSameString()
        {
            var stringParser = BasicParsers.GetStringParser("whatever");
            try
            {
                var result = stringParser.Parse("hatever");
            }
            catch (Exception exception)
            {
                Assert.AreEqual("Expected \"whatever\", got \"hatever\".", exception.Message);
            }
        }
    }
}
