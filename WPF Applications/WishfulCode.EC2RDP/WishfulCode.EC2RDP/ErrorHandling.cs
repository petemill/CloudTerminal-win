using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace WishfulCode.EC2RDP
{
    public static class ErrorHandling
    {
        public static void LogError(Exception ex, string key)
        {
            //TODO: better error handling
            Trace.WriteLine(String.Concat("[App Error] ",key,": ", ex.Message));
        }
    }
}
