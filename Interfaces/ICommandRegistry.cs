namespace BasicShell.Interfaces;

public interface ICommandRegistry
{
    void RegisterCommand(ICommand command);
    ICommand? GetCommand(string name);
    bool IsBuiltInCommand(string name);
}