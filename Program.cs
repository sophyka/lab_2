using System;
using System.Collections.Generic;


class Program
{
    static void Main()
    {
        Turtle myTurtle = new Turtle();
        myTurtle.PrintMenu();
        Console.WriteLine(myTurtle);
        while (true)
        {
            string command = Console.ReadLine();

            if (command == "exit")
            {   
                Console.Clear();
                break;
            }
            Console.Clear();
            myTurtle.PrintMenu();
            myTurtle.ProcessCommand(command);
            Console.WriteLine($"\n{myTurtle} | Prev command is [{command}]");
        }
    }
}
