using BasicShell.Core;
using BasicShell.Interfaces;

namespace BasicShell.Commands;

public class ExternalCommand(string name, string path) : ICommand
{
    private readonly string _path = path;

    public string Name { get; } = name;

    public async Task ExecuteAsync(string[] args, TextWriter output, TextWriter error)
    {
        var (outStr, errStr, exitCode) = await ProcessRunner.RunProgramAsync(_path, args);
        if (!string.IsNullOrEmpty(outStr))
            output.WriteLine(outStr);
        if (!string.IsNullOrEmpty(errStr))
            error.WriteLine(errStr);
    }
}