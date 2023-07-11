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
        // Get the path to the database file and save it for connection open/close later
        static string connectString = Path.Combine(AppContext.BaseDirectory, "FlashcardsDB.db");
        static string connectionString = $"Data Source= {connectString}";
        
        internal int getChoice()
        {
            Console.WriteLine("Enter the ID of the stack you want to edit.");
            var userChoice = Console.ReadLine();
            var result = int.TryParse(userChoice, out var choice);
            if (result == false)
            {
                Console.WriteLine("Enter in an integer. Please try again.");
                getChoice();
            }
            return choice;
        }

        internal void showCreateMenu()
        {
            var stackChoice = validateChoice();
            Console.Clear();
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
                    addCard(stackChoice);
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

        internal int validateChoice()
        {
            int intChoice;
            var success = false;
            do  // Use a do-while loop to ensure that we get the correct input
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

                    bool result = int.TryParse(userChoice, out intChoice);

                    var isFound = false;

                    foreach (var stack in checkStacks)
                    {
                        if (intChoice == stack.StackID)
                        {
                            success = true;
                        }
                    }
                    if (isFound == false)
                    {
                        Console.WriteLine("\nThe ID entered does not exist. Please try again.");
                    }
                }
            }   while (!success);
            return intChoice;
        }

        internal void addCard(int choice)
        {
            Console.WriteLine("What would you like to write on the front?");
            var frontDesc = Console.ReadLine();

            Console.WriteLine("What would you like to write on the back?");
            var backDesc = Console.ReadLine();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    $"INSERT INTO Flashcard(StackID, FrontDesc, BackDesc) VALUES ('{choice}', '{frontDesc}', '{backDesc}')";

                tableCmd.ExecuteNonQuery();
                connection.Close();
            }
            showCreateMenu();
        }
    }
}
