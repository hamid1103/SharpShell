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
            if (input == "exit")
            {
                break;
            }
            
            //split[0] should be the command.
            switch (split[0])
            {
                case "cd":
                    if (split.Count == 1)
                    {
                        Console.WriteLine("cd: invalid path");
                        break;
                    }
                    Commands.CDCommand(split[1]);
                    break;
                case "pwd":
                    string workDir = Directory.GetCurrentDirectory();
                    Console.WriteLine(workDir);
                    break;
                case "type":
                    tester.TypeCommand(split[1]);
                    break;
                case "echo":
                    Commands.EchoCommand(input[5..]);
                    break;
                default:
                    List<string> args = ParseArgs(input);
                    
                    TestCommandResults resl = tester.TestCommand(split[0]);
                    
                    if (resl.Found)
                    {

                        var psi = new ProcessStartInfo
                        {
                            FileName = resl.ExecutablePath,
                            UseShellExecute = false
                        };

                        for (int i = 1; i < args.Count; i++)
                        {
                            psi.ArgumentList.Add(args[i]);
                        }
                        
                        /*var psi = new ProcessStartInfo {

                            FileName = "/bin/sh",
                            Arguments =
                                $"-c \"exec -a {args[0]} {resl.ExecutablePath}{args}\"",
                            UseShellExecute = false
                        };*/

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
        List<string> args = new();
        var current = new StringBuilder();

        bool inSingleQuote = false;

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            //Can't directly compare char to string
            if (c == Convert.ToChar("\'"))
            {
                inSingleQuote = !inSingleQuote;
                continue;
            }

            if (char.IsWhiteSpace(c) && !inSingleQuote)
            {
                if (current.Length > 0)
                {
                    args.Add(current.ToString());
                    current.Clear();
                }
            }
            else
            {
                current.Append(c);
            }
        }

        if (current.Length > 0)
        {
            args.Add(current.ToString());
        }
        
        
        return args;
    }
}
