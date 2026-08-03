using System;

namespace BulletDevil.Utilities;

public static class Utils
{
	public static void ThrowWarning(object sender, string message)
	{
		Console.Write($"({sender}) ");
		Console.ForegroundColor = ConsoleColor.Yellow;
		Console.WriteLine(message);
		Console.ResetColor();
	}

	public static void ThrowWarning(string sender, string message)
	{
		Console.Write($"({sender}) ");
		Console.ForegroundColor = ConsoleColor.Yellow;
		Console.WriteLine(message);
		Console.ResetColor();
	}

	public static void ThrowError(object sender, string message)
	{
		Console.Write($"({sender}) ");
		throw new Exception("\x1B[38;2;255;0;0m" + message + "\x1B[0m");
	}
}