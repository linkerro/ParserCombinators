using System;
using System.Collections.Generic;
using System.Linq;

namespace ParserCombinators
{
    public static class ParserExtensions
    {
        public static Result<TInput> Parse<TInput>(this Parser<TInput> parser, TInput input)
        {
            return parser.Func(input);
        }

        /// <summary>
        /// Returns a parser that transforms the results of the mapped parser.
        /// </summary>
        /// <typeparam name="TInput">The type of the parser input.</typeparam>
        /// <param name="parser">The parser providing the result to be transformed.</param>
        /// <param name="mapper">A function that transforms the results of a parser.</param>
        /// <returns>A parser that transforms the results of the mapped parser.</returns>
        public static Parser<TInput> Map<TInput>(this Parser<TInput> parser,
            Func<object, object> mapper)
        {
            Parser<TInput> mappedParser = new Parser<TInput>()
            {
                Func = input =>
                {
                    var result = parser.Func(input);
                    if (result.GetType() == typeof(Error<string>))
                    {
                        return result;
                    }
                    var mappedResult = mapper(result.Output);
                    return new Result<TInput>() { Rest = result.Rest, Output = mappedResult };
                },
                ExpectedInput = $"{parser.ExpectedInput}"
            };
            return mappedParser.Name($"mapped {parser.GetName()}");
        }

        /// <summary>
        /// Generates a parsers that matches one or more times.
        /// </summary>
        /// <param name="parser">The parser that is to be matched one or more times.</param>
        /// <returns>A parser that matches one or more times.</returns>
        public static Parser<string> OneOrMany(this Parser<string> parser)
        {
            Parser<string> mappedParser = new Parser<string>();

            mappedParser.Func = input =>
            {
                var test = mappedParser;

                var rest = input;
                var resultCount = 0;
                var matches = new List<object>();

                while (!string.IsNullOrEmpty(rest))
                {
                    var result = parser.Parse(rest);
                    resultCount += 1;

                    if ((result as Error<string>) != null)
                    {
                        if (resultCount == 1)
                        {
                            return result;
                        }
                        return new Result<string>
                        {
                            Rest = rest,
                            Output = matches.Where(m => m != null).ToList()
                        };
                    }

                    rest = result.Rest;
                    var output = result.Output as IEnumerable<object>;
                    if (output != null)
                    {
                        matches.AddRange(output);
                    }
                }
                return new Result<string>
                {
                    Rest = rest,
                    Output = matches.Where(m => m != null).ToList()
                };
            };
            return mappedParser;
        }

        /// <summary>
        /// Generates a parser that matches 1 or 0 matches of the parent parser.
        /// </summary>
        /// <param name="parser">The parent parser.</param>
        /// <returns>A parser that matches 1 or 0 matches of the parent parser.</returns>
        public static Parser<string> Optional(this Parser<string> parser)
        {
            Parser<string> optionalCombinator = new Parser<string>
            {
                Func = input =>
                {
                    var result = parser.Func(input);
                    if (result.GetType() == typeof(Error<string>))
                    {
                        return new Result<string>
                        {
                            Output = null,
                            Rest = input
                        };
                    }
                    return result;
                },
                ExpectedInput = $"{parser.ExpectedInput}(optional)"
            };
            return optionalCombinator.Name($"optional {parser.GetName()}");
        }

    }
}