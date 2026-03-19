using System.Diagnostics;

class Program
{
    static void Main()
    {
        // Source - https://stackoverflow.com/a/185214
        // Posted by Patrick Desjardins, modified by community. See post 'Timeline' for change history
        // Retrieved 2026-03-19, License - CC BY-SA 4.0
        var PATHValue = System.Environment.GetEnvironmentVariable("PATH");
        
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
                case "type":
                    tester.TypeCommand(split[1]);
                    break;
                case "echo":
                    Console.Write($"{input[5..]}\n");
                    break;
                default:
                    TestCommandResults resl = tester.TestCommand(split[0]);
                    if (resl.Found)
                    {
                        if (split.Count > 2)
                        {
                            Process.Start(resl.ExecutablePath, split[1..]);
                        }
                        else
                        {
                            Process.Start(resl.ExecutablePath);
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
