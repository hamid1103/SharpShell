using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public class Tester
{
    private readonly string PATH;
    private List<string> PathSearchLocations;
    public Tester(string PATH)
    {
        this.PATH = PATH;
        PathSearchLocations = PATH.Split(":").ToList();
    }

    public TestCommandResults TestCommand(string input)
    {
        TestCommandResults result = new();
        result.Found = false;
        foreach (var searchLocation in PathSearchLocations)
        {
            string executePath = Path.Combine(searchLocation, input);
            if (Path.Exists(executePath))
            {
                UnixFileMode mode = File.GetUnixFileMode(executePath);
                bool canExecute =
                    (mode & (UnixFileMode.UserExecute | UnixFileMode.GroupExecute |
                             UnixFileMode.OtherExecute)) != 0;
                if (canExecute)
                {
                    result.ExecutablePath = executePath;
                    result.PathDir = searchLocation;
                    result.Found = true;
                    return result;
                }
            }
        }
        return result;
    }

    public void TypeCommand(string input)
    {
        switch (input)
        {
            case "pwd":
            case "type":
            case "exit":
            case "echo":
                Console.WriteLine($"{input} is a shell builtin");
                break;
            default:
                //Check if command is executable.
                bool found = false;
                foreach (var searchLocation in PathSearchLocations)
                {
                    string executePath = Path.Combine(searchLocation, input);
                    if (Path.Exists(executePath))
                    {
                        UnixFileMode mode = File.GetUnixFileMode(executePath);
                        bool canExecute =
                            (mode & (UnixFileMode.UserExecute | UnixFileMode.GroupExecute |
                                     UnixFileMode.OtherExecute)) != 0;
                        if (canExecute)
                        {
                            Console.WriteLine($"{input} is {executePath}");
                            found = true;
                            break;
                        }
                    }
                }

                if (!found)
                {
                    //If not, not found
                    Console.WriteLine($"{input}: not found");   
                }
                break;
        }
    }
}