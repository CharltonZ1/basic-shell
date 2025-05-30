using BasicShell.Commands;
using BasicShell.Core;
using BasicShell.Interfaces;

namespace BasicShell;

class Program
{
    static async Task Main(string[] args)
    {
        var registry = new CommandRegistry();
        registry.RegisterCommand(new ExitCommand());
        registry.RegisterCommand(new EchoCommand());
        registry.RegisterCommand(new TypeCommand(registry));

        while (true)
        {
            Console.Write("$ ");
            string? input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
                continue;

            input = input.Trim();
            string[] parameters = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string commandName = parameters[0];
            string[] cmdArgs = parameters.Length > 1 ? parameters[1..] : [];

            ICommand? command = registry.GetCommand(commandName);
            if (command != null)
            {
                await command.ExecuteAsync(cmdArgs, Console.Out, Console.Error);
            }
            else
            {
                Console.WriteLine($"{commandName}: command not found");
            }
        }
    }
}