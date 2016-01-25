using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ParserCombinators
{
    public class Parser<TInput>
    {
        public Func<TInput, Result<TInput>> Func;
        public string ExpectedInput;

        public static Parser<TInput> operator +(
            Parser<TInput> parser1, Parser<TInput> parser2)
        {
            return new Parser<TInput>
            {
                Func = input =>
                {
                    var result = parser1.Func(input);
                    if (result.GetType() == typeof(Error<string>))
                    {
                        return result;
                    }
                    var result2 = parser2.Func(result.Rest);
                    if (result2.GetType() == typeof(Error<string>))
                    {
                        return result2;
                    }
                    var output1 = result.Output as IEnumerable<object> ?? new List<object>() { result.Output };
                    var output2 = result2.Output as IEnumerable<object> ?? new List<object>() { result2.Output };

                    var compositeResult = new List<IEnumerable<object>>() { output1, output2 }.SelectMany(x => x).Where(x => x != null).ToList();
                    return new Result<TInput>() { Output = compositeResult, Rest = result2.Rest };
                }
            };
        }

        public static Parser<TInput> operator |(
            Parser<TInput> parser1, Parser<TInput> parser2)
        {
            return new Parser<TInput>
            {
                Func = input =>
                {
                    var result = parser1.Func(input);
                    if (result.GetType() != typeof(Error<TInput>))
                    {
                        return result;
                    }
                    result = parser2.Func(input);
                    if (result.GetType() != typeof(Error<TInput>))
                    {
                        return result;
                    }
                    return new Error<TInput>()
                    {
                        Message = $"Expected <parser1.ExpectedInput|parser2.ExpectedInput>",
                        Expected = parser1.ExpectedInput.ToString() + "|" + parser2.ExpectedInput.ToString(),
                        Actual = input.ToString()
                    };
                }
            };
        }
    }

    public static class ParserExtensions
    {
        public static Result<TInput> Parse<TInput>(this Parser<TInput> parser, TInput input)
        {
            return parser.Func(input);
        }

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
                }
            };
            return mappedParser;
        }

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
                        return new Result<string> { Output = outputs.SelectMany(x => x).Where(x => x != null).ToList() };
                    }
                    return new Error<string>
                    {
                        Expected = parser.ExpectedInput,
                        Actual = parser.ExpectedInput
                    };
                }
            };
            return mappedParser;
        }

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
                }
            };
            return optionalCombinator;
        }

    }
}