class Program
{
    static void Main()
    {
        while (true)
        {
            Console.Write("$ ");
            string input = Console.ReadLine();
            if (input == "exit")
            {
                break;
            }else if (input.StartsWith("echo "))
            {
                Console.Write($"{input[5..]}\n");
            }
            else
            {
                switch (input)
                {
                    default:
                        Console.WriteLine($"{input}: command not found");
                        break;
                }
            }
            
        }
    }
}
