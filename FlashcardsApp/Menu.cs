using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace FlashcardsApp
{
    internal class Menu
    {
        // Get the path to the database file and save it for connection open/close later
        static string connectString = Path.Combine(AppContext.BaseDirectory, "FlashcardsDB.db");
        static string connectionString = $"Data Source= {connectString}";
        createStackMenu createStack = new createStackMenu();

        internal void showMenu()
        {
            bool closeApp = false;
            while (closeApp == false)
            { 
                Console.Clear();
                Console.WriteLine("-------------------------------------------------");
                Console.WriteLine("Welcome to your flashcards!");
                Console.WriteLine("Enter 1 to create a new stack.");
                Console.WriteLine("Enter 2 to view existing stacks.");
                Console.WriteLine("Enter 3 to edit an existing stack.");
                Console.WriteLine("Enter 4 to delete an existing stack.");
                Console.WriteLine("Enter 5 to exit.");
                Console.WriteLine("-------------------------------------------------");

                var userInput = Console.ReadLine();

                switch (userInput)
                {
                    case "1":
                        createNewStack();
                        break;
                    case "2":
                        showStacks();
                        Console.WriteLine("Enter any key to return to the menu.");
                        Console.ReadLine();
                        showMenu();
                        break;
                    case "3":
                        showStacks();
                        createStack.showCreateMenu();
                        break;
                    case "4":
                        showStacks();
                        deleteStack();
                        break;
                    case "5":
                        Console.WriteLine("Exiting...");
                        closeApp = true;
                        break;
                    default:
                        Console.WriteLine("Invalid input. Enter any key to try again.");
                        Console.ReadLine();
                        break;
                }
            }
        }

        internal void createNewStack()
        {
            Console.Clear();
            Console.WriteLine("Enter in the name of your new stack: ");
            var stackName = Console.ReadLine(); // Save user input to insert into database later

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    $"INSERT INTO Stack(StackName) VALUES ('{stackName}')"; // Use string interpolation to insert name of stack

                tableCmd.ExecuteNonQuery(); // We are not querying anything from table, simply inserting a record into the table
                connection.Close();
            }
        }

        internal void showStacks()
        {
            Console.Clear();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    "SELECT * FROM Stack";

                List<Stack> stacks = new(); // Create a list of the Stack type to populate with data later

                SqliteDataReader reader = tableCmd.ExecuteReader(); // Initialize reader to retrieve data

                if (reader.HasRows) // Ensure there is data in the table
                {
                    while (reader.Read())
                    {
                        stacks.Add(     // Add entries to the list
                            new Stack
                            {
                                StackID = reader.GetInt32(0),   // According to database table, StackID and StackName are in positions 0 & 1, respectively                                                                 * 
                                StackName = reader.GetString(1)     
                            });
                    }
                } else Console.WriteLine("No rows found.");
                connection.Close();

                // Print out all records in the list
                Console.WriteLine("-----------------------------------");
                foreach (var stack in stacks)
                {
                    Console.WriteLine($"StackID: {stack.StackID}");
                    Console.WriteLine($"Stack name: {stack.StackName}\n");
                }
                Console.WriteLine("-----------------------------------");
            }
        }

        internal void deleteStack()
        {
            Console.WriteLine("Enter in the ID of the stack you want to delete.");
            var deleteChoice = Console.ReadLine();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    $"DELETE FROM Stack WHERE StackID = '{deleteChoice}'";

                int rowCount = tableCmd.ExecuteNonQuery();
                
                if (rowCount == 0)
                {
                    Console.WriteLine($"No stack with the ID {deleteChoice} was found.");
                    deleteStack();
                }
            }
            Console.WriteLine($"the stack with the ID {deleteChoice} was deleted.");
            showMenu();
        }
    }
}
