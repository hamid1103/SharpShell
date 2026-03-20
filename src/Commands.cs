using System.Text.RegularExpressions;

public class Commands
{
    public static void CDCommand(string Dir)
    {
        //Path handling comes here. Later. I think.
        string FixedDir;
        FixedDir = Dir.Replace("~", Environment.GetEnvironmentVariable("HOME"));
        //check if valid firts
        if (Directory.Exists(FixedDir))
        {
            
            Directory.SetCurrentDirectory(FixedDir);
        }
        else if (File.Exists(FixedDir))
        {
            Console.WriteLine($"cd: {FixedDir}: Path is a file");
        }
        else
        {
            Console.WriteLine($"cd: {FixedDir}: No such file or directory");
        }
    }

    public static string Escape(string arg)
    {
        return $"'{arg.Replace("'", "'\\''")}'";
    }
    
    public static void EchoCommand(List<string> argString)
    {
        
        //Old one
        Console.Write(string.Join(" ", argString.Skip(1)) + "\n");
    }
    
    public static void EchoCommand(string argString)
    {
        
        //Old one
        Console.Write(string.Join(" ", argString) + "\n");
    }
}