namespace FirstTests.Utilities;

public static class Log
{
    public static void Info(string msg) =>
        TestContext.Progress.WriteLine($"[{DateTime.Now:HH:mm:ss}] [INFO]  {msg}");

    public static void Error(string msg) =>
        TestContext.Progress.WriteLine($"[{DateTime.Now:HH:mm:ss}] [ERROR] {msg}");
}