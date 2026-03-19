class Program
{
    static void Main()
    {
        while (true)
        {
            Console.WriteLine("$ ");
            string input = Console.ReadLine();
            Console.WriteLine($"{input}: command not found");   
        }
    }
}
