using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashcardsApp
{
    internal class createStackMenu
    {
        internal void showCreateMenu()
        {
            Console.Clear();
            Console.WriteLine("Enter in the name of your new stack: ");
            var stackName = Console.ReadLine();

            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("Enter 1 to create a new card.");
            Console.WriteLine("Enter 2 to view existing cards.");
            Console.WriteLine("Enter 3 to delete an existing card.");
            Console.WriteLine("Enter 4 when you are finished.");
            Console.WriteLine("-------------------------------------------------");

            var userInput = Console.ReadLine();

            switch (userInput)
            {
                case "1":
                    Console.WriteLine("Creating new card...");
                    break;
                case "2":
                    Console.WriteLine("Viewing existing cards...");
                    break;
                case "3":
                    Console.WriteLine("Deleting an existing card...");
                    break;
                case "4":
                    Console.WriteLine("Finishing...");
                    break;
                default:
                    Console.WriteLine("Invalid input. Enter any key to try again.");
                    break;
            }
        }
    }
}
