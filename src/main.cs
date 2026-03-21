using System.Diagnostics;
using System.Text;

class Program
{
    static void Main()
    {
        // Source - https://stackoverflow.com/a/185214
        // Posted by Patrick Desjardins, modified by community. See post 'Timeline' for change history
        // Retrieved 2026-03-19, License - CC BY-SA 4.0
        var PATHValue = System.Environment.GetEnvironmentVariable("PATH");
        var UserHomePath = Environment.GetEnvironmentVariable("HOME");

        Tester tester = new Tester(PATHValue);

        while (true)
        {
            bool RedirectStdOut = false;
            bool RedirectErrorOut = false;
            string redirectOutputFile = "";
            string redirectErrorFile = "";
            int stdOutRedirectionIndex = 0;
            int stdErrorRedirectionIndex = 0;

            Console.Write("$ ");
            string input = Console.ReadLine();
            List<string> args = ParseArgs(input);

            if (input == "exit")
            {
                break;
            }

            if (args.Contains(">") || args.Contains("1>"))
            {
                RedirectStdOut = true;
                stdOutRedirectionIndex = args.FindIndex(arg => arg is ">" or "1>");
                redirectOutputFile = args[stdOutRedirectionIndex + 1];
                Console.WriteLine(
                    "stdout redirect index = " + stdOutRedirectionIndex + $"filename {redirectOutputFile}");
            }

            if (args.Contains("2>"))
            {
                RedirectErrorOut = true;
                stdErrorRedirectionIndex = args.FindIndex(arg => arg is "2>");
                redirectErrorFile = args[stdErrorRedirectionIndex + 1];
                Console.WriteLine("error redirect index = " + stdErrorRedirectionIndex +
                                  $" filename {redirectErrorFile}");
            }

            if (args.Count > 0)
            {
                //split[0] should be the command.
                switch (args[0])
                {
                    case "cd":
                        if (args.Count == 1)
                        {
                            Console.WriteLine("cd: invalid path");
                            break;
                        }

                        Commands.CDCommand(args[1]);
                        break;
                    case "pwd":
                        string workDir = Directory.GetCurrentDirectory();
                        Console.WriteLine(workDir);
                        break;
                    case "type":
                        tester.TypeCommand(args[1]);
                        break;
                    case "echo":
                        Commands.EchoCommand(args, out var echoOutput);
                        if (RedirectStdOut)
                        {
                            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), redirectOutputFile),
                                echoOutput);
                        }
                        else
                        {
                            Console.WriteLine(echoOutput);
                        }

                        break;
                    default:
                        TestCommandResults resl = tester.TestCommand(args[0]);
                        if (resl.Found)
                        {
                            var psi = new ProcessStartInfo
                            {
                                FileName = args[0],
                                UseShellExecute = false
                            };

                            string stdout = "";
                            string stdErr = "";

                            if (RedirectStdOut || RedirectErrorOut)
                            {
                                int cutoffIndex = RedirectStdOut
                                    ? stdOutRedirectionIndex
                                    : stdErrorRedirectionIndex;
                                for (int i = 1; i < cutoffIndex; i++)
                                {
                                    psi.ArgumentList.Add(args[i]);
                                    Console.WriteLine(args[i]);
                                }

                                if (RedirectStdOut)
                                {
                                    psi.RedirectStandardOutput = true;
                                }

                                if (RedirectErrorOut)
                                {
                                    psi.RedirectStandardError = true;
                                }

                                Process prc = Process.Start(psi);
                                stdErr = RedirectErrorOut ? prc.StandardError.ReadToEnd() : "";
                                stdout = RedirectStdOut ? prc.StandardOutput.ReadToEnd() : "";
                                prc?.WaitForExit();
                            }
                            else
                            {
                                for (int i = 1; i < args.Count; i++)
                                {
                                    psi.ArgumentList.Add(args[i]);
                                }

                                Process.Start(psi)?.WaitForExit();
                            }

                            if (!string.IsNullOrEmpty(stdErr))
                            {
                                File.WriteAllText(redirectErrorFile, stdErr);
                            }

                            if (!string.IsNullOrEmpty(stdout))
                            {
                                File.WriteAllText(args[stdOutRedirectionIndex + 1], stdout);
                            }
                        }
                        else
                        {
                            Console.WriteLine($"{input}: command not found");
                        }

                        break;
                }
            }
        }
    }

    public static List<string> ParseArgs(string input)
    {
        StringUtils stringUtils = new StringUtils();
        List<string> args = new();
        var current = new StringBuilder();

        bool inSingleQuote = false;
        bool inDoubleQuote = false;
        bool isEscaping = false;

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            //Can't directly compare char to string
            if (c == Convert.ToChar("\'") && !inDoubleQuote && !isEscaping)
            {
                inSingleQuote = !inSingleQuote;
                continue;
            }

            if (c == Convert.ToChar("\\") && !inSingleQuote && !isEscaping)
            {
                isEscaping = true;
                continue;
            }

            if (c == Convert.ToChar("\"") && !isEscaping && !inSingleQuote)
            {
                inDoubleQuote = !inDoubleQuote;
                continue;
            }

            if (char.IsWhiteSpace(c) && !inSingleQuote)
            {
                if (isEscaping)
                {
                    current.Append(c);
                    isEscaping = !isEscaping;
                }
                else if (inDoubleQuote)
                {
                    //if in double quote, just add the whitespace
                    current.Append(c);
                }
                else if (current.Length > 0)
                {
                    args.Add(current.ToString());
                    current.Clear();
                }
            }
            else
            {
                if (!isEscaping)
                {
                    current.Append(c);
                }
                else
                {
                    //is escaping
                    if (stringUtils.HasSpecialMeaning(c))
                    {
                        if ((i + 1) > input.Length)
                        {
                            string isDoubleCheckString = c.ToString() + input[i + 1];
                            Console.WriteLine("Escaping, with special meaning: " + isDoubleCheckString);
                            if (stringUtils.IsDoubleSpecial(isDoubleCheckString))
                            {
                                current.Append(c);
                                current.Append(input[i + 1]);
                                i++;
                            }
                        }
                        else
                        {
                            current.Append(c);
                        }
                    }
                    else
                    {
                        current.Append(c);
                    }

                    isEscaping = !isEscaping;
                }
            }
        }

        if (current.Length > 0)
        {
            args.Add(current.ToString());
        }


        return args;
    }
}