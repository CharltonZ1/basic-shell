using BasicShell.Interfaces;
using BasicShell.Commands;

namespace BasicShell.Core;

public class CommandRegistry : ICommandRegistry
{
    private readonly Dictionary<string, ICommand> _commands = new(StringComparer.OrdinalIgnoreCase);

    public void RegisterCommand(ICommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (string.IsNullOrWhiteSpace(command.Name)) throw new ArgumentException("Command name cannot be null or whitespace.", nameof(command));

        _commands[command.Name] = command;
    }

    public ICommand? GetCommand(string name)
    {
        if (_commands.TryGetValue(name, out var command))
        {
            return command;
        }

        string? path = ProcessRunner.GetPath(name);
        return path != null ? new ExternalCommand(name, path) : null;
    }

    public bool IsBuiltInCommand(string name)
    {
        return _commands.ContainsKey(name);
    }
}