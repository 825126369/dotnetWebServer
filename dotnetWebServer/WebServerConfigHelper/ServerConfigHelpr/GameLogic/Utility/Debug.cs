using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace GameLogic
{
    public static class Debug
    {
        static Debug()
        {
#if DEBUG
            Console.Clear();
#endif
        }

        private static string GetTimeStr()
        {
            return DateTime.Now.ToLongTimeString();
        }

        public static void PrintStackTraceInfo()
        {
#if DEBUG
            StackTrace st = new StackTrace(true);
            Console.WriteLine(GetTimeStr() + " : " + st.ToString());
#endif
        }

        private static void PrintInfo(object message)
        {
#if DEBUG
            Console.WriteLine(GetTimeStr() + " : " + message);
#endif
        }

        public static void Log(object message)
        {
#if DEBUG
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            PrintInfo(message.ToString());
#endif
        }

        public static void LogError(string message)
        {
#if DEBUG
            Console.ForegroundColor = ConsoleColor.DarkRed;
            PrintInfo(message.ToString());
            PrintStackTraceInfo();
#endif
        }

        public static void LogWarning(object message)
        {
#if DEBUG
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            PrintInfo(message.ToString());
#endif
        }

        public static void Assert(bool bTrue, object message = null)
        {
#if DEBUG
            if (!bTrue)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                if (message == null)
                {
                    PrintInfo("Assert Error");
                }
                else
                {
                    PrintInfo("Assert Error: " + message);
                }

                PrintStackTraceInfo();
            }
#endif
        }
    }
}
