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


        public static string GetExceptionDetails(Exception ex)
        {
            if (ex == null) throw new ArgumentNullException("ex");
            var sb = new StringBuilder();
            FillExceptionDetails(sb, ex);
            return sb.ToString();
        }
        static void FillExceptionDetails(StringBuilder sb, Exception ex)
        {
            sb.AppendLine(ex.GetType() + ": " + ex.Message);
            sb.AppendLine(ex.StackTrace);
            if (ex.InnerException != null)
            {
                sb.AppendLine("------INNER EXCEPTION------");
                FillExceptionDetails(sb, ex.InnerException);
            }
            // TODO when this assembly is recompiled in .NET4, we should treat AggregateExceptions as a special case, and output all InnerExceptions.
        }

    }
}
