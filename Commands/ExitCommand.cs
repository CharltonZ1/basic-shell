using BasicShell.Interfaces;

namespace BasicShell.Commands;

public class ExitCommand : ICommand
{
    public string Name => "exit";

    public Task ExecuteAsync(string[] args, TextWriter output, TextWriter errorOutput)
    {
        if (args.Length > 0 && args[0] == "0")
        {
            Environment.Exit(0);
        }
        else
        {
            errorOutput.WriteLine("Usage: exit 0");
        }

        return Task.CompletedTask;
    }
}
