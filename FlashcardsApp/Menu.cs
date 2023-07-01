using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashcardsApp
{
    class Menu
    {
        createStackMenu createStack = new();
        internal void showMenu()
        {
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("Welcome to your flashcards!");
            Console.WriteLine("Enter 1 to create a new stack.");
            Console.WriteLine("Enter 2 to view existing stacks.");
            Console.WriteLine("Enter 3 to exit.");
            Console.WriteLine("-------------------------------------------------");

            var userInput = Console.ReadLine();

            switch (userInput)
            {
                case "1":
                    Console.WriteLine("Creating new stack...");
                    createStack.showCreateMenu();
                    break;
                case "2":
                    Console.WriteLine("Viewing existing stacks...");
                    break;
                case "3":
                    Console.WriteLine("Exiting...");
                    break;
                default:
                    Console.WriteLine("Invalid input. Enter any key to try again.");
                    Console.ReadLine();
                    break;

            }
        }
    }
}
