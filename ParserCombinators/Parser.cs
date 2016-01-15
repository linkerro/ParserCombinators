using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ParserCombinators
{
    public delegate Result<TInput, TValue> Parser<TInput, TValue>(TInput input);

    public static class BasicParsers
    {
        public static Parser<string, string> GetStringParser(string match)
        {
            return input =>
            {
                if (input.StartsWith(match))
                {
                    return new Result<string, string> {Output = match, Rest = input.Substring(match.Length)};
                }
                return new Error<string, string> {Message = $"Expected \"{match}\", got \"{input}\"."};
            };
        }

        public static Parser<string, string> GetRegexParser(Regex expression)
        {
            return input =>
            {
                var match = expression.Match(input);

                if (match.Success)
                {
                    return new Result<string, string> {Output = match.Value,Rest = input.Substring(match.Index+match.Length)};
                }
                return new Error<string, string> {Message = $"Expected match on '{expression.ToString()}', got '{input}'."};
            };
        }
    }

    public static class ParserExtensions
    {
        public static Result<TInput,TOutput> Parse<TInput, TOutput>(this Parser<TInput, TOutput> parser, TInput input)
        {
            return parser(input);
        }
    }
}