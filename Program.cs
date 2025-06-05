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
        bool inDoubleQuotes = false;

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            if (inSingleQuotes)
            {
                if (c == '\'')
                {
                    inSingleQuotes = false;
                }
                else
                {
                    currentArg.Append(c);
                }
            }
            else if (inDoubleQuotes)
            {
                if (c == '"')
                {
                    inDoubleQuotes = false;
                }
                else if (c == '\\' && i + 1 < input.Length)
                {
                    char next = input[i + 1];
                    if (next == '$' || next == '`' || next == '"' || next == '\\' || next == '\n')
                    {
                        currentArg.Append(next);
                        i++; // skip escaped character
                    }
                    else
                    {
                        // backslash before other chars is kept
                        currentArg.Append(c);
                        currentArg.Append(next);
                        i++;
                    }
                }
                else
                {
                    currentArg.Append(c);
                }
            }
            else
            {
                if (char.IsWhiteSpace(c))
                {
                    if (currentArg.Length > 0)
                    {
                        args.Add(currentArg.ToString());
                        currentArg.Clear();
                    }
                }
                else if (c == '\'')
                {
                    inSingleQuotes = true;
                }
                else if (c == '"')
                {
                    inDoubleQuotes = true;
                }
                else if (c == '\\' && i + 1 < input.Length)
                {
                    char next = input[i + 1];

                    if (next == '\n')
                    {
                        i++; // skip newline
                    }
                    else
                    {
                        currentArg.Append(next);
                        i++; // skip escaped character
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
        string[] cmdArgs = args.Count > 1 ? args.GetRange(1, args.Count - 1).ToArray() : [];

        return (commandName, cmdArgs);
    }

}