using Azure.Core;
using System.Diagnostics;

namespace OJudge.Services
{
    public class CppCompilerService
    {
        public static async Task<string> Compile(string Code) {
            var codesDir = @"C:\Users\Amin Stors\Documents\AAA Учёба\Мои проекты\OJudge\Codes";
            
            try
            {
                var cppPath = Path.Combine(codesDir, "program.cpp");
                var exePath = Path.Combine(codesDir, "program.exe");

                await System.IO.File.WriteAllTextAsync(cppPath, Code);

                var compile = new ProcessStartInfo("g++", $"\"{cppPath}\" -o \"{exePath}\"")
                {
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var compileProc = Process.Start(compile);
                var errorOutput = await compileProc.StandardError.ReadToEndAsync();
                await compileProc.WaitForExitAsync();

                if (compileProc.ExitCode != 0)
                {
                    return "error: " + errorOutput;
                }

                return "ok";
            } finally {
            }
        }
    }
}
