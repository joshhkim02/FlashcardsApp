using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace FlashcardsApp
{
    class Menu
    {
        static string connectString = Path.Combine(AppContext.BaseDirectory, "FlashcardsDB.db");
        static string connectionString = $"Data Source= {connectString}";
        createStackMenu createStack = new();

        internal void showMenu()
        {
            Console.Clear();
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("Welcome to your flashcards!");
            Console.WriteLine("Enter 1 to create a new stack.");
            Console.WriteLine("Enter 2 to view existing stacks.");
            Console.WriteLine("Enter 3 to edit an existing stack.");
            Console.WriteLine("Enter 4 to exit.");
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
                    editStack();
                    
                    break;
                case "4":
                    addCard();
                    break;
                default:
                    Console.WriteLine("Invalid input. Enter any key to try again.");
                    Console.ReadLine();
                    break;

            }
        }

        internal void addCard()
        {
            Console.WriteLine("Enter in the ID of the stack you want to add this to.");
            var stackID = Console.ReadLine(); 

            Console.WriteLine("What would you like to write on the front?");
            var frontDesc = Console.ReadLine();

            Console.WriteLine("What would you like to write on the back?");
            var backDesc = Console.ReadLine();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    $"INSERT INTO Flashcard(StackID, FrontDesc, BackDesc) VALUES ('{stackID}', '{frontDesc}', '{backDesc}')";

                tableCmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        internal void createNewStack()
        {
            Console.Clear();
            Console.WriteLine("Enter in the name of your new stack: ");
            var stackName = Console.ReadLine();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    $"INSERT INTO Stack(StackName) VALUES ('{stackName}')";

                tableCmd.ExecuteNonQuery();
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

                List<Stack> stacks = new();

                SqliteDataReader reader = tableCmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        stacks.Add(
                            new Stack
                            {
                                StackID = reader.GetInt32(0),
                                StackName = reader.GetString(1)
                            });
                    }
                } else Console.WriteLine("No rows found.");
                connection.Close();

                Console.WriteLine("-----------------------------------");
                foreach (var stack in stacks)
                {
                    Console.WriteLine($"StackID: {stack.StackID}");
                    Console.WriteLine($"Stack name: {stack.StackName}\n");
                }
                Console.WriteLine("-----------------------------------");
            }
        }

        internal void editStack()
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

                bool doesExist = false;

                foreach (var stack in checkStacks)
                {
                    if (intChoice == stack.StackID)
                    {
                        Console.WriteLine("ID entered does exist!");
                        doesExist = true;
                    }
                }    
                if (doesExist == false)
                {
                    Console.WriteLine("ID does not exist, please try again.");
                    editStack();
                }    
                
            }
        }
    }
}
