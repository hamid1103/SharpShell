public class CDCommandClass
{
    public static void CDCommand(string Dir)
    {
        //Path handling comes here. Later. I think.
        
        //check if valid firts
        if (Directory.Exists(Dir))
        {
            Directory.SetCurrentDirectory(Dir);
        }
        else if (File.Exists(Dir))
        {
            Console.WriteLine($"cd: {Dir}: Path is a file");
        }
        else
        {
            Console.WriteLine($"cd: {Dir}: No such file or directory");
        }
    }
}