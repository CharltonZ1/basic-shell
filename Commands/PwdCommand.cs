using BasicShell.Interfaces;

namespace BasicShell.Commands;

public class PwdCommand : ICommand
{
    public string Name => "pwd";

    public Task ExecuteAsync(string[] args, TextWriter output, TextWriter errorOutput)
    {
        if (args.Length > 0)
        {
            errorOutput.WriteLine("Usage: pwd");
            return Task.CompletedTask;
        }

        string currentDirectory = Environment.CurrentDirectory;
        output.WriteLine(currentDirectory);

        return Task.CompletedTask;
    }
}
