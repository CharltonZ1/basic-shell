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
            var (commandName, cmdArgs, redirects) = ParseInput(input);

            ICommand? command = registry.GetCommand(commandName);
            if (command != null)
            {
                TextWriter stdout = Console.Out;
                TextWriter stderr = Console.Error;

                StreamWriter? outFile = null;
                StreamWriter? errFile = null;

                try
                {
                    if (redirects.TryGetValue(1, out var stdoutRedir))
                    {
                        outFile = new StreamWriter(stdoutRedir.Path, append: stdoutRedir.Append)
                        {
                            AutoFlush = true
                        };
                        stdout = outFile;
                    }

                    if (redirects.TryGetValue(2, out var stderrRedir))
                    {
                        errFile = new StreamWriter(stderrRedir.Path, append: stderrRedir.Append)
                        {
                            AutoFlush = true
                        };
                        stderr = errFile;
                    }

                    await command.ExecuteAsync(cmdArgs, stdout, stderr);
                }
                finally
                {
                    outFile?.Dispose();
                    errFile?.Dispose();
                }
            }
            else
            {
                Console.WriteLine($"{commandName}: command not found");
            }
        }
    }

    private static (string CommandName, string[] Arguments, Dictionary<int, (string Path, bool Append)> Redirects) ParseInput(string input)
    {
        var args = new List<string>();
        var redirects = new Dictionary<int, (string Path, bool Append)>();
        var currentArg = new StringBuilder();

        bool inSingleQuotes = false;
        bool inDoubleQuotes = false;

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            if (inSingleQuotes)
            {
                if (c == '\'')
                    inSingleQuotes = false;
                else
                    currentArg.Append(c);
            }
            else if (inDoubleQuotes)
            {
                if (c == '"')
                    inDoubleQuotes = false;
                else if (c == '\\' && i + 1 < input.Length)
                {
                    char next = input[i + 1];
                    if (next is '$' or '`' or '"' or '\\' or '\n')
                    {
                        currentArg.Append(next);
                        i++;
                    }
                    else
                    {
                        currentArg.Append(c).Append(next);
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
                    if (input[i + 1] == '\n')
                    {
                        i++; // line continuation
                    }
                    else
                    {
                        currentArg.Append(input[++i]);
                    }
                }
                else if ((c == '>' || c == '<') && currentArg.Length <= 1)
                {

                    bool isAppend = false;

                    if (c == '>' && i + 1 < input.Length && input[i + 1] == '>')
                    {
                        isAppend = true;
                        i++; // consume second >
                    }

                    // Handle redirection operators: >, 1>, 2>
                    int fd = 1; // default to stdout
                    if (currentArg.Length == 1 && char.IsDigit(currentArg[0]))
                    {
                        fd = currentArg[0] - '0';
                        currentArg.Clear();
                    }

                    // skip current '>' or '<'
                    i++;
                    while (i < input.Length && char.IsWhiteSpace(input[i])) i++; // skip whitespace

                    var filename = new StringBuilder();
                    //bool quoted = false;
                    if (i < input.Length && (input[i] == '"' || input[i] == '\''))
                    {
                        //quoted = true;
                        char quoteType = input[i++];
                        while (i < input.Length && input[i] != quoteType)
                            filename.Append(input[i++]);
                    }
                    else
                    {
                        while (i < input.Length && !char.IsWhiteSpace(input[i]))
                            filename.Append(input[i++]);
                        i--; // step back to balance next loop increment
                    }

                    redirects[fd] = (filename.ToString(), isAppend);
                }
                else
                {
                    currentArg.Append(c);
                }
            }
        }

        if (currentArg.Length > 0)
            args.Add(currentArg.ToString());

        string commandName = args.Count > 0 ? args[0] : string.Empty;
        string[] cmdArgs = args.Count > 1 ? [.. args.GetRange(1, args.Count - 1)] : [];

        return (commandName, cmdArgs, redirects);
    }


}