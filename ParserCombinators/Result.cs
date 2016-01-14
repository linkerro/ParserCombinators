using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserCombinators
{
    public class Result<TInput, TOutput>
    {
        public TOutput Output;
        public TInput Rest;
    }

    public class Error<TInput, TOutput> : Result<TInput, TOutput>
    {
        public string Message;
    }
}
