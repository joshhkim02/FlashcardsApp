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
                    createNewStack();
                    //createStack.showCreateMenu();
                    break;
                case "2":
                    Console.WriteLine("Viewing existing stacks...");
                    testDatabase();
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

        internal void createNewStack()
        {
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

        internal void testDatabase()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    $"SELECT * FROM Flashcard";

                List<Flashcard> personData = new();

                SqliteDataReader reader = tableCmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        personData.Add(
                            new Flashcard
                            {
                                CardID = reader.GetInt32(0),
                                StackID = reader.GetInt32(1),
                                FrontDesc = reader.GetString(2),
                                BackDesc = reader.GetString(3)
                            });
                    }
                }
                else Console.WriteLine("No rows found");
                connection.Close();

                Console.WriteLine("-----------------------------------");
                foreach (Flashcard card in personData)
                {
                    Console.WriteLine($"{card.CardID} - {card.StackID} - {card.FrontDesc} - {card.BackDesc}");
                }
                Console.WriteLine("-----------------------------------");
            }
        }
    }
}
