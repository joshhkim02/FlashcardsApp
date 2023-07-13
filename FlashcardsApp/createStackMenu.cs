using Microsoft.Data.Sqlite;
using System;
using System.Collections;
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
        // Establish global variables to ensure the same stack is edited after every operation
        internal int timesRun = 0;
        internal int stackChoice = 0;
        
        internal void showCreateMenu()
        {
            // Makes sure that after every operation the same stack is chosen
            if (timesRun == 0)
            {
                int getChoice = validateChoice();
                stackChoice = getChoice;
                timesRun++;
            }

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
                    viewCards(stackChoice);
                    Console.WriteLine("Enter any key to go back.");
                    Console.ReadLine();
                    showCreateMenu();
                    break;
                case "3":
                    viewCards(stackChoice);
                    deleteCard(stackChoice);
                    break;
                case "4":
                    // Leaving this empty simply finishes the showCreateMenu(), going back to the menu class
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
            Console.Clear();
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

        internal void viewCards(int choice)
        {
            Console.Clear();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    $"SELECT CardID, FrontDesc, BackDesc FROM Flashcard WHERE StackID = '{choice}'";

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
                } else Console.WriteLine("No rows found.");
                connection.Close();

                Console.WriteLine("-----------------------------------");
                foreach (var card in flashcards)
                {
                    Console.WriteLine($"CardID: {card.CardID}");
                    Console.WriteLine($"Front Description: {card.FrontDesc}");
                    Console.WriteLine($"Back Description: {card.BackDesc}\n");
                }
                Console.WriteLine("-----------------------------------");
            }
        }

        internal void deleteCard(int choice)
        {
            Console.WriteLine("Enter the ID of the card you want to delete.");
            var deleteChoice = Console.ReadLine();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    $"DELETE FROM FlashCard WHERE CardID = '{deleteChoice}'";

                int rowCount = tableCmd.ExecuteNonQuery();  // Assign the number of rows affected to a variable

                if (rowCount == 0)
                {
                    Console.WriteLine($"Record with ID {deleteChoice} does not exist.");
                    deleteCard(choice);
                }    
            }
            Console.WriteLine($"Record with ID {deleteChoice} was deleted."); // If query goes through, let user know card was deleted
            showCreateMenu();
        }
    }
}
