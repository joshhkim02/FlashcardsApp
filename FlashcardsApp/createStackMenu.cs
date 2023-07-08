using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashcardsApp
{
    internal class createStackMenu
    {
        static string connectString = Path.Combine(AppContext.BaseDirectory, "FlashcardsDB.db");
        static string connectionString = $"Data Source= {connectString}";
        internal void showCreateMenu()
        {
            bool closeApp = false;
            while (closeApp == false)
            {
                validateChoice();
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
        internal void validateChoice()
        {
            Console.WriteLine("Enter the ID of the stack you would like to edit.");
            var userChoice = Console.ReadLine();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    $"SELECT StackID from Stack";

                List<Stack> checkStacks = new();

                SqliteDataReader reader = tableCmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        checkStacks.Add(
                            new Stack
                            {
                                StackID = reader.GetInt32(0),
                            });
                    }
                }
                else Console.WriteLine("No rows found.");
                connection.Close();

                int.TryParse(userChoice, out var intChoice);

                bool isFound = false;

                foreach (var stack in checkStacks)
                {
                    if (intChoice == stack.StackID)
                    {
                        isFound = true;
                    }
                }
                if (isFound == true)
                {
                    Console.WriteLine("The ID entered exists.");
                }
                else if (isFound == false)
                {
                    Console.WriteLine("The ID entered does not exist.");
                }
            }
        }
    }
}
