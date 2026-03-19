class Program
{
    static void Main()
    {
        Tester tester = new Tester();
        
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
                    tester.TestCommand(split[1]);
                    break;
                case "echo":
                    Console.Write($"{input[5..]}\n");
                    break;
                default:
                    Console.WriteLine($"{input}: command not found");
                    break;
            }
            
        }
    }
}
