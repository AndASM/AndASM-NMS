using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

// https://gist.github.com/tomzorz/6142d69852f831fb5393654c90a1f22e

namespace AndASM_NMS.Windows
{
	public static class ColorConsole
	{
		private const int  STD_OUTPUT_HANDLE                  = -11;
		private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
		private const uint DISABLE_NEWLINE_AUTO_RETURN        = 0x0008;

		public const char Esc = '\u001b';

		public static readonly string NewLine         = Environment.NewLine;
		public static readonly string Normal          = $"{Esc}[0m";
		public static readonly string Underline       = $"{Esc}[4m";
		public static readonly string NoUnderline     = $"{Esc}[24m";
		public static readonly string FgRed           = $"{Esc}[31m";
		public static readonly string FgYellow        = $"{Esc}[33m";
		public static readonly string FgBrightRed     = $"{Esc}[91m";
		public static readonly string FgBrightGreen   = $"{Esc}[92m";
		public static readonly string FgBrightYellow  = $"{Esc}[93m";
		public static readonly string FgBrightMagenta = $"{Esc}[95m";
		public static readonly string FgBrightCyan    = $"{Esc}[96m";
		public static readonly string FgBrightWhite   = $"{Esc}[97m";

		private static readonly StringBuilder ErrorLogBuilder;

		static ColorConsole()
		{
			// Enable VT100
			var stdOutHandle = GetStdHandle(STD_OUTPUT_HANDLE);
			GetConsoleMode(stdOutHandle, out var ConsoleMode);
			SetConsoleMode(stdOutHandle,
						   ConsoleMode | ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN);
			ErrorLogBuilder = new StringBuilder();
		}


		[DllImport("kernel32.dll")]
		private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

		[DllImport("kernel32.dll")]
		private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

		[DllImport("kernel32.dll")]
		private static extern IntPtr GetStdHandle(int nStdHandle);

		private static void Write(string text)
		{
			ErrorLogBuilder.Append(text);
			Console.Write(text);
		}

		private static void WriteLine(string text)
		{
			Write($"{text}{NewLine}");
		}

		public static void WriteInfo(string info)
		{
			WriteLine($"{Normal}{info}");
		}

		public static void WriteNameValue(string name, string value)
		{
			WriteLine($"{Normal}{name}:{NewLine}" +
					  $"{FgBrightWhite}\t{value}{Normal}");
		}

		public static void WriteKeyTabValue(string name, string value)
		{
			WriteLine($"{FgBrightGreen}{name}\t{FgBrightYellow}{value}{Normal}");
		}

		public static void WriteBlankLine()
		{
			WriteLine("");
		}

		public static void WriteTryExcept(string actionText, string success, Action actionCode)
		{
			try
			{
				WriteInfo(actionText);
				actionCode();
				WriteLine($"{FgBrightGreen}{success}{Normal}");
			}
			catch (Exception e)
			{
				WriteException(e.ToString());
			}
		}

		public static void WriteException(string errorText)
		{
			var errorLogFile = new FileInfo("ErrorLog.txt");

			WriteLine($"{FgBrightRed}Error!");
			WriteLine(
				$"Please check {Normal}{Underline}https://github.com/AndASM/ANMSMEMSPC/issues{NoUnderline}{FgBrightRed} for this issue.");
			WriteLine("If it hasn't been reported, please click the New Issue button and report it.");
			WriteLine("Please include the error log available here:");
			WriteLine($"{FgBrightWhite}{errorLogFile.FullName}{FgBrightRed}");
			WriteBlankLine();

			WriteLine("Important technical details:");
			WriteLine($"{FgRed}{errorText}{Normal}");

			if (errorLogFile.Exists) errorLogFile.Delete();
			using var errorOutput = errorLogFile.CreateText();
			errorOutput.WriteLine(
				Regex.Replace(
					ErrorLogBuilder.ToString(),
					@"\u001b\[\d+m",
					""
				));
			errorOutput.Flush();

			ConfirmQuit();
			Environment.Exit(1);
		}

		public static void ConfirmQuit()
		{
			Console.Write("Press any key to exit.");
			Console.ReadKey();
		}
	}
}