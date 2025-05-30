using BasicShell.Core;
using BasicShell.Interfaces;

namespace BasicShell.Commands;

public class TypeCommand(ICommandRegistry commandRegistry) : ICommand
{
    private readonly ICommandRegistry _commandRegistry = commandRegistry;

    public string Name => "type";

    public Task ExecuteAsync(string[] args, TextWriter output, TextWriter error)
    {
        if (args.Length == 0)
        {
            error.WriteLine("type is a shell builtin");
            return Task.CompletedTask;
        }

        string target = args[0];
        if (_commandRegistry.IsBuiltInCommand(target))
        {
            output.WriteLine($"{target} is a shell builtin");
        }
        else
        {
            string? path = ProcessRunner.GetPath(target);
            if (path != null)
            {
                output.WriteLine($"{target} is {path}");
            }
            else
            {
                output.WriteLine($"{target}: not found");
            }
        }
        return Task.CompletedTask;
    }

}
