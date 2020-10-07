using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Digify.Micro
{
    public struct Part
    {
 
        public static readonly Part Value = new Part();

        public static readonly Task<Part> Task = System.Threading.Tasks.Task.FromResult(Value);
    }
}
