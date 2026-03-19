public class Tester
{
    public Tester()
    {
        
    }

    public void TestCommand(string input)
    {
        switch (input)
        {
            case "type":
            case "exit":
            case "echo":
                Console.WriteLine($"{input} is a shell builtin");
                break;
            default:
                Console.WriteLine($"{input}: not found");
                break;
        }
    }
}