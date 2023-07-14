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
                Console.WriteLine("Enter 5 to start studying a stack.");
                Console.WriteLine("Enter 6 to exit.");
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
                        showStacks();
                        studyStack();
                        break;
                    case "6":
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
            Console.WriteLine($"\nThe stack with the ID {deleteChoice} was deleted.");
            Console.WriteLine("Enter any key to go back to the menu.");
            Console.ReadLine();
        }

        internal void studyStack()
        {
            Console.WriteLine("Enter in the ID of the stack you want to study.");
            var studyChoice = Console.ReadLine();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    $"SELECT CardID, FrontDesc, BackDesc FROM Flashcard WHERE StackID = '{studyChoice}'";

                List<Flashcard> flashcards = new();

                SqliteDataReader reader = tableCmd.ExecuteReader();

                var cardOrdinal = reader.GetOrdinal("CardID");
                var frontOrdinal = reader.GetOrdinal("FrontDesc");
                var backOrdinal = reader.GetOrdinal("BackDesc");

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        flashcards.Add(
                            new Flashcard
                            {
                                // GetOrdinal gets the index to use (index is also called the ordinal value of the column)
                                CardID = reader.GetInt32(cardOrdinal),
                                FrontDesc = reader.GetString(frontOrdinal),
                                BackDesc = reader.GetString(backOrdinal)
                            });
                    }
                }
                else Console.WriteLine("No rows found.");
                connection.Close();

                Console.WriteLine("-----------------------------------");

                int score = 0;

                for (int i = 0; i < flashcards.Count; i++)
                {
                    Console.WriteLine(flashcards[i].FrontDesc);
                    Console.Write("Answer here: ");
                    var answer = Console.ReadLine();

                    if (answer == flashcards[i].BackDesc)
                    {
                        Console.WriteLine($"Correct! The answer is: {flashcards[i].BackDesc}");
                        Console.WriteLine("Enter any key to go to the next card.");
                        score++;
                    }
                    else
                    {
                        Console.WriteLine($"Incorrect. The answer is: {flashcards[i].BackDesc}");
                        Console.WriteLine("Enter any key to go to the next card.");
                    }
                    Console.ReadLine();
                }

                Console.WriteLine($"You've gone through the stack! Your score is: {score}");
                Console.WriteLine("-----------------------------------");

                Console.ReadLine();
            }
        }
    }
}
;