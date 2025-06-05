using BasicShell.Commands;
using BasicShell.Core;
using BasicShell.Interfaces;
using System.Text;

namespace BasicShell;

class Program
{
    static async Task Main(string[] args)
    {
        var registry = new CommandRegistry();
        registry.RegisterCommand(new ExitCommand());
        registry.RegisterCommand(new EchoCommand());
        registry.RegisterCommand(new TypeCommand(registry));
        registry.RegisterCommand(new PwdCommand());
        registry.RegisterCommand(new CdCommand());

        while (true)
        {
            Console.Write("$ ");
            string? input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
                continue;

            input = input.Trim();
            var (commandName, cmdArgs) = ParseInput(input);

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

    private static (string CommandName, string[] Arguments) ParseInput(string input)
    {
        var args = new List<string>();
        var currentArg = new StringBuilder();
        bool inSingleQuotes = false;

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            if (inSingleQuotes)
            {
                if (c == '\'')
                {
                    // Check if the next char starts a new quoted section without whitespace
                    int next = i + 1;
                    if (next < input.Length && input[next] == '\'')
                    {
                        // Escaped single quote (two single quotes in a row)
                        //currentArg.Append('\'');
                        i++;
                    }
                    else
                    {
                        inSingleQuotes = false;
                        // Do NOT add currentArg to args yet — may be concatenated
                    }
                }
                else
                {
                    currentArg.Append(c);
                }
            }
            else
            {
                if (c == '\'')
                {
                    inSingleQuotes = true;
                    // If currentArg has content, stay in it — we're likely concatenating
                }
                else if (char.IsWhiteSpace(c))
                {
                    if (currentArg.Length > 0)
                    {
                        args.Add(currentArg.ToString());
                        currentArg.Clear();
                    }
                }
                else
                {
                    currentArg.Append(c);
                }
            }
        }

        if (currentArg.Length > 0)
        {
            args.Add(currentArg.ToString());
        }

        string commandName = args.Count > 0 ? args[0] : string.Empty;
        string[] cmdArgs = args.Count > 1 ? [.. args.GetRange(1, args.Count - 1)] : [];
        return (commandName, cmdArgs);
    }

}