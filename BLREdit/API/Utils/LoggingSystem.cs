using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace BLREdit
{
    public static class LoggingSystem
    {
        public static Stopwatch Log(string info, LogTypes level = LogTypes.Info, string newLine = "\n")
        {
            if ((App.Settings?.EnableDebugging ?? false) && (App.Settings.LogLevel.HasFlag(LogTypes.All) || App.Settings.LogLevel.HasFlag(level)))
            {
                var now = DateTime.Now;
                Trace.Write("[" + now + "]"+level.ToString()+":" + info + newLine);
                return Stopwatch.StartNew();
            }
            return null;
        }

        public static void Append(Stopwatch watch, string finish = "")
        {
            if (App.Settings.EnableDebugging)
            {
                string output = finish;
                if (watch != null)
                {
                    output += " Done! in " + (watch.ElapsedTicks / (double)Stopwatch.Frequency).ToString("0.00000") + "s";
                }
                Trace.WriteLine(output);
            }
        }

        public static string ObjectToTextWall<T>(T obj)
        {
            StringBuilder sb = new();
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            var fields = obj.GetType().GetFields(flags);
            var props = obj.GetType().GetProperties(flags);
            sb.Append('{');
            foreach (FieldInfo field in fields)
            {
                sb.AppendFormat(" {0}:{1},", field.Name, field.GetValue(obj));
            }
            foreach (PropertyInfo prop in props)
            {
                sb.AppendFormat(" {0}:{1},", prop.Name, prop.GetValue(obj));
            }
            sb.Append(" }");
            return sb.ToString();
        }
    }

    [Flags]
    public enum LogTypes 
    { 
        None = 0,
        All = 1,
        Info = 2,
        Warning = 4,
        Error = 8,
        Fatal = 16,
        Timing = 32,
    }
}
