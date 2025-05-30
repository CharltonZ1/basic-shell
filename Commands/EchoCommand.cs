using BasicShell.Interfaces;

namespace BasicShell.Commands;

public class EchoCommand : ICommand
{
    public string Name => "echo";

    public Task ExecuteAsync(string[] args, TextWriter output, TextWriter errorOutput)
    {
        if (args.Length > 0)
        {
            output.WriteLine(string.Join(" ", args));
        }
        else
        {
            errorOutput.WriteLine("Usage: echo <message>");
        }

        return Task.CompletedTask;
    }
}
