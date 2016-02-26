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
        /// Gerates a parser that can matches one or more instances of its parent parser.
        /// </summary>
        /// <param name="parser">The parent parser to be matched.</param>
        /// <param name="separator">Any posible separator.</param>
        /// <returns></returns>
        public static Parser<string> OneOrMany(this Parser<string> parser, string separator)
        {
            Parser<string> mappedParser = new Parser<string>()
            {
                Func = input =>
                {
                    var rest = input;
                    var outputs = new List<IEnumerable<object>>();
                    rest = separator + rest;

                    while (!string.IsNullOrEmpty(rest))
                    {
                        rest = rest.Substring(separator.Length, rest.Length - separator.Length);

                        var result = parser.Func(rest);
                        if (result.GetType() != typeof(Error<string>))
                        {
                            outputs.Add(result.Output as IEnumerable<object> ?? new List<object>() { result.Output });
                            rest = result.Rest;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (outputs.Count > 0)
                    {
                        return new Result<string>
                        {
                            Output = outputs.SelectMany(x => x).Where(x => x != null).ToList()
                        };
                    }
                    return new Error<string>
                    {
                        Expected = parser.ExpectedInput,
                        Actual = input
                    };
                },
                ExpectedInput = $"{parser.ExpectedInput}(one or many)"
            };
            return mappedParser.Name($"oneOrMany {parser.GetName()}");
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
                    if (result.GetType() == typeof (Error<string>))
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