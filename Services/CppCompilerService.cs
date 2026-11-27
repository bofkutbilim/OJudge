using System.Diagnostics;

namespace OJudge.Services
{
    public class CppCompilerService
    {
        public static async Task<string?> Compile(string code, string exePath)
        {
            var cppPath = Path.ChangeExtension(exePath, ".cpp");
            await File.WriteAllTextAsync(cppPath, code);
            var compile = new ProcessStartInfo("g++")
            {
                WorkingDirectory = Path.GetDirectoryName(exePath)!,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            compile.ArgumentList.Add(cppPath);
            compile.ArgumentList.Add("-o");
            compile.ArgumentList.Add(exePath);
            JudgeProcessEnvironment.LimitToToolchain(compile);
            using var compiler = Process.Start(compile) ?? throw new IOException("Could not start g++.");
            var errors = await compiler.StandardError.ReadToEndAsync();
            await compiler.WaitForExitAsync();
            return compiler.ExitCode == 0 ? null : errors;
        }
    }
}
