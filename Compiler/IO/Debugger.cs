using System;

namespace Compiler.IO
{

	public static class Debugger
	{
        public const bool DEBUG = true;

        public static void WriteDebuggingInfo(string message)
        {
            if (DEBUG)
                System.Console.WriteLine($"DEBUGGING INFO: {message}");
        }
	}
}
