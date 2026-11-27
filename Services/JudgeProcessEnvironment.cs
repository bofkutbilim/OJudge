using System.Diagnostics;

namespace OJudge.Services;

internal static class JudgeProcessEnvironment
{
    // Compiler processes and submitted programs must not inherit the API's signing key or connection string.
    public static void LimitToToolchain(ProcessStartInfo process)
    {
        process.Environment.Clear();
        foreach (var name in new[] { "PATH", "SystemRoot", "SystemDrive", "ComSpec", "PATHEXT", "TEMP", "TMP", "TMPDIR", "LANG" })
        {
            if (Environment.GetEnvironmentVariable(name) is string value)
                process.Environment[name] = value;
        }
    }
}
