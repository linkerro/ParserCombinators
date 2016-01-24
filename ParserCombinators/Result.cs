using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserCombinators
{
    public class Result<TInput>
    {
        public object Output;
        public TInput Rest;
    }

    public class Error<TInput> : Result<TInput>
    {
        public string Message;
        public string Expected;
        public string Actual;
    }
}
