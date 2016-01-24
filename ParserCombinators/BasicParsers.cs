using System;
using System.Text.RegularExpressions;

namespace ParserCombinators
{
    public static class BasicParsers
    {
        public static Parser<string> GetStringParser(string match)
        {
            return new Parser<string>
            {
                ExpectedInput = match,
                Func =
                    input =>
                        input.StartsWith(match)
                            ? new Result<string> { Output = null, Rest = input.Substring(match.Length) }
                            : new Error<string>
                            {
                                Message = $"Expected \"{match}\", got \"{input}\".",
                                Expected = match,
                                Actual = input
                            }
            };
        }

        public static Parser<string> GetRegexParser(Regex expression)
        {
            return new Parser<string>
            {
                ExpectedInput = expression.ToString(),
                Func = input =>
                {
                    var match = expression.Match(input);

                    return match.Success && match.Index==0
                        ? new Result<string>
                        {
                            Output = match.Value,
                            Rest = input.Substring(match.Index + match.Length)
                        }
                        : new Error<string>
                        {
                            Message = $"Expected match on '{expression.ToString()}', got '{input}'.",
                            Expected = expression.ToString(),
                            Actual = input
                        };
                }
            };
        }

        public static Func<string, Parser<string>> S = GetStringParser;

        public static Parser<string> R(string expression)
        {
            return GetRegexParser(new Regex(expression));
        }  
    }
}