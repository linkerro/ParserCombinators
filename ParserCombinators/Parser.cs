using System;
using System.Text.RegularExpressions;

namespace ParserCombinators
{
    public class Parser<TInput, TOutput>
    {
        public Func<TInput, Result<TInput, TOutput>> Func;

        public static Parser<TInput, TOutput> operator +(
            Parser<TInput, TOutput> parser1, Parser<TInput, TOutput> parser2)
        {
            return new Parser<TInput, TOutput>
            {
                Func = input =>
                {
                    var result = parser1.Func(input);
                    return parser2.Func(result.Rest);
                }
            };
        }
    }

    public static class BasicParsers
    {
        public static Parser<string, string> GetStringParser(string match)
        {
            return new Parser<string, string>
            {
                Func =
                    input =>
                        input.StartsWith(match)
                            ? new Result<string, string> {Output = match, Rest = input.Substring(match.Length)}
                            : new Error<string, string> {Message = $"Expected \"{match}\", got \"{input}\"."}
            };
        }

        public static Parser<string, string> GetRegexParser(Regex expression)
        {
            return new Parser<string, string>
            {
                Func = input =>
                {
                    var match = expression.Match(input);

                    return match.Success
                        ? new Result<string, string>
                        {
                            Output = match.Value,
                            Rest = input.Substring(match.Index + match.Length)
                        }
                        : new Error<string, string>
                        {
                            Message = $"Expected match on '{expression.ToString()}', got '{input}'."
                        };
                }
            };
        }
    }

    public static class ParserExtensions
    {
        public static Result<TInput, TOutput> Parse<TInput, TOutput>(this Parser<TInput, TOutput> parser, TInput input)
        {
            return parser.Func(input);
        }
    }
}