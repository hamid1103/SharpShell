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

    public static void EchoCommand(List<string> argString, out string output)
    {
        bool redirecting = false;
        List<string> echoStringList = new();
        foreach (var arg in argString)
        {
            if (arg == ">" || arg == "1>" || arg == "2>")
            {
                int lastIndex = echoStringList.FindLastIndex(x => x == echoStringList.Last());
                echoStringList[lastIndex] = echoStringList[lastIndex] + "\n";
                redirecting = true;
                break;
            }

            echoStringList.Add(arg);
        }

        output = string.Join(" ", echoStringList.Skip(1));
        if (!redirecting) output += "\n";
    }
}