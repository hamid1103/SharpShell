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
                    var argsSplit = input.Split(split[0]);
                    string argsString = "";
                    if (resl.Found)
                    {
                        string args = "";

                        for (int i = 1; i < split.Count; i++) {
                            args += $" \"{split[i]}\"";
                        }

                        var psi = new ProcessStartInfo {

                            FileName = "/bin/sh",
                            Arguments =

                                $"-c \"exec -a {split[0]} {resl.ExecutablePath}{args}\"",

                            UseShellExecute = false
                        };
                        
                        //Commented for now. This uses original shell which feels a bit like cheating.
                        /*var psi = new ProcessStartInfo { FileName = "/bin/sh",

                            Arguments = $"-c \"{input}\"",

                            UseShellExecute = false };*/

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
}
