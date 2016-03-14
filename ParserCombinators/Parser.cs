using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ParserCombinators
{
    public class Parser<TInput>
    {
        public Func<TInput, Result<TInput>> Func;
        public string ExpectedInput;
        private string _name;

        private void Add(Parser<TInput> parser1, Parser<TInput> parser2)
        {
            Func= input =>
            {
                var test = _name;
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
            };
            ExpectedInput = $"{parser1.ExpectedInput} + {parser2.ExpectedInput}";
        }

        private void Or(Parser<TInput> parser1, Parser<TInput> parser2)
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
            };
            ExpectedInput = $"{parser1.ExpectedInput} | {parser2.ExpectedInput}";
        }

        public static Parser<TInput> operator +(
            Parser<TInput> parser1, Parser<TInput> parser2)
        {
            var parser = new Parser<TInput>();
            parser.Add(parser1,parser2);
            return parser;
        }

        public static Parser<TInput> operator |(
            Parser<TInput> parser1, Parser<TInput> parser2)
        {
            var parser = new Parser<TInput>();
            parser.Or(parser1,parser2);
            return parser;
        }

        public Parser<TInput> Name(string name)
        {
            _name = name;
            return this;
        }

        public string GetName()
        {
            return _name;
        }
    }
}