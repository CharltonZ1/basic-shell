namespace BasicShell.Interfaces;

public interface ICommand
{
    string Name { get; }
    Task ExecuteAsync(string[] args, TextWriter output, TextWriter errorOutput);
}