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
            Console.Write("$ ");
            string input = Console.ReadLine();
            List<string> split = input.Split().ToList();
            List<string> args = ParseArgs(input);
            
            if (input == "exit")
            {
                break;
            }
            
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
                    Commands.EchoCommand(args);
                    break;
                default:
                    TestCommandResults resl = tester.TestCommand(split[0]);
                    
                    if (resl.Found)
                    {

                        var psi = new ProcessStartInfo
                        {
                            FileName = args[0],
                            UseShellExecute = false
                        };

                        for (int i = 1; i < args.Count; i++)
                        {
                            psi.ArgumentList.Add(args[i]);
                        }
                        
                        Process.Start(psi)?.WaitForExit();


                    }
                    else
                    {
                        Console.WriteLine($"{input}: command not found");
                    }
                    break;
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

            if (c == Convert.ToChar("\\") && !inDoubleQuote && !inSingleQuote && !isEscaping)
            {
                isEscaping = true;
                continue;
            }
            
            if (c == Convert.ToChar("\"") && !isEscaping)
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
                }else if (inDoubleQuote)
                {
                    //if in double quote, just add the whitespace
                    current.Append(c);
                }else if (current.Length > 0)
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
                            string isDoubleCheckString = c.ToString() + input[i+1];
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
                        if (input[i].ToString() == "\\")
                        {
                            current.Append("\\\\");
                        }
                        else
                        {
                            current.Append(c);
                        }
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
