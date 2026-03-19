class Program
{
    static void Main()
    {
        while (true)
        {
            Console.Write("$ ");
            string input = Console.ReadLine();
            if (input == "exit")
                break;
            Console.WriteLine($"{input}: command not found");
        }
    }
}
